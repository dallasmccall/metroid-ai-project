using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace MetroidAIGameLibrary.player
{
    // Model of player to guide generation, including accuracy, damage taken, etc.
    public class PlayerModel
    {
        public string statPath { get; set; }

        public Dictionary<string, float> playerStats = new Dictionary<string, float>()
        {
            {"accuracy", 0.5f},         // (# hit) / (# fired)
            {"shots", 100f},            // total shots fired
            {"damageTaken", 10f},       // total damage taken
            {"damageDone", 8f},         // total damage inflicted to enemies
            {"roomsExplored", 0.5f},    // % of total space explored
            {"roomsVisited", 10f}      // total rooms entered
            
        };

        public PlayerModel()
        { 
        }

        public PlayerModel(Dictionary<string,float> playerStats)
        {
            foreach (var key in playerStats.Keys.ToList())
            {
                if (playerStats.ContainsKey(key))
                {
                    playerStats[key] = playerStats[key];
                }
            }
        }

        // reset the playerModel to all values of 0
        public void resetModel()
        {
            foreach (var key in playerStats.Keys.ToList())
            {
                playerStats[key] = 0f;
            }
        }

        // set a single stat for the player model
        public bool setStat(string key, float value)
        {
            if (playerStats.ContainsKey(key))
            {
                playerStats[key] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        // set multiple player stats; ignores anything not in the model
        public bool setStatDict(Dictionary<string, float> stats)
        {
            foreach (var key in stats.Keys.ToList())
            {
                if (playerStats.ContainsKey(key))
                {
                    playerStats[key] = stats[key];
                }
                /* do I need this?
            else
            {
                // stat not in dictionary
                return false;
            }
                 */
            }
            return true;
        }

        // get a single stat for the player model
        public float getStat(string key)
        {
            if (playerStats.ContainsKey(key))
            {
                return playerStats[key];
            }
            else
            {
                // should this just throw an exception?
                return -100000f;
            }
        }

        public Dictionary<string,float> getStatDict(List<string> keys)
        {
            Dictionary<string,float> calledStats = new Dictionary<string,float>();
            foreach (string key in keys)
            {
                calledStats[key] = playerStats[key];
            }
            return calledStats;
        }

        public Dictionary<string,float> getAllStats()
        {
            return playerStats;
        }
    }

    public class ModelFormula
    {
        public string prefixFormula { get; set; }
        public string postfixFormula { get; set; }
        public Stack<String> formula { get; set; }
        public List<string> formulaList { get; set; }
 
        public ModelFormula()
        {
            prefixFormula = "";
            postfixFormula = "";
            formula = new Stack<string>();
            formulaList = new List<string>();
        }

        // constructor from appropriately formatted xml
        // means a long sequence of <Item> #/variable name </Item>
        // assumes postfix notation
        public ModelFormula(string xmlFile)
        {
            string str = "";
            formula = new Stack<string>();
            using (XmlTextReader reader = new XmlTextReader(xmlFile))
            {
                reader.Read();
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.Name.ToString() == "Item"))
                    {
                        reader.Read();
                        formula.Push(reader.Value.ToString());
                    }
                }
            }
            formulaList = formula.ToList();
            // reverse stack order from prefix to postfix
            formula = new Stack<string>(formula);
            prefixFormula = string.Join("", formulaList.ToArray());
            formulaList.Reverse();
            postfixFormula = string.Join("", formulaList.ToArray());   
        }

        // List should be a postfix expression with each character being a value, variable, or operator
        public ModelFormula(List<string> form)
        {
            formulaList = new List<string>(form);
            formula = new Stack<string>(form);
            // copied in reverse order, so switch back
            formula = new Stack<string>(formula);
            postfixFormula = string.Join("", form.ToArray());
            form.Reverse();
            prefixFormula = string.Join("", form.ToArray());
        }

        // indicate whether input string type is "prefix" or "postfix", form is formula
        // only acceptable variables are single character
        public ModelFormula(string type, string form)
        {
            if (type == "prefix")
            {
                // set up prefix and postfix strings
                prefixFormula = form;
                char[] rev = form.ToCharArray();
                Array.Reverse(rev);
                postfixFormula = new string(rev);
                
                // create stack representation
                formula = new Stack<string>();
                foreach (char c in form.ToCharArray())
                {
                    formula.Push(c.ToString());
                }
                formulaList = formula.ToList<string>();
            }
            else if (type == "postfix")
            {
                // set up prefix and postfix strings
                postfixFormula = form;
                char[] rev = form.ToCharArray();
                Array.Reverse(rev);
                prefixFormula = new string(rev);

                // create stack representation
                formula = new Stack<string>();
                char[] form2 = form.ToCharArray();
                for (int i = form2.Length - 1; i >= 0; i--)
                {
                    formula.Push(form2[i].ToString());
                }
                formulaList = formula.ToList<string>();
            }
        }
        
        public string PrefixString()
        {
            return prefixFormula;
        }

        public string PostfixString()
        {
            return postfixFormula;
        }

        // postfix formula evaluation; input dictionary is used to lookup variable values
        public float evalFormula(Dictionary<string, float> variables)
        {
            Stack<string> calc = new Stack<string>();
            Stack<string> temp = new Stack<string>(formula.Reverse());
            string val = "";
            float v1 = 0f;
            float v2 = 0f;
            while (temp.Count() > 0)
            {
                val = temp.Pop();
                if (val == "^")
                {
                    int exp = System.Convert.ToInt32(calc.Pop());
                    v1 = (float)System.Convert.ToDouble(calc.Pop());
                    v1 = (float)Math.Pow(v1, exp);
                    calc.Push(v1.ToString());
                }
                else if (val == "*")
                {
                    v1 = (float)System.Convert.ToDouble(calc.Pop());
                    v2 = (float)System.Convert.ToDouble(calc.Pop());
                    v1 *= v2;
                    calc.Push(v1.ToString());
                }
                else if (val == "/")
                {
                    v1 = (float)System.Convert.ToDouble(calc.Pop());
                    v2 = (float)System.Convert.ToDouble(calc.Pop());
                    v1 /= v2;
                    calc.Push(v1.ToString());
                }
                else if (val == "+")
                {
                    v1 = (float)System.Convert.ToDouble(calc.Pop());
                    v2 = (float)System.Convert.ToDouble(calc.Pop());
                    v1 += v2;
                    calc.Push(v1.ToString());
                }
                else if (val == "-")
                {
                    v1 = (float)System.Convert.ToDouble(calc.Pop());
                    v2 = (float)System.Convert.ToDouble(calc.Pop());
                    v1 -= v2;
                    calc.Push(v1.ToString());
                }

                else if (variables.ContainsKey(val))
                {
                    v1 = variables[val];
                    calc.Push(v1.ToString());
                }

                else
                {
                    calc.Push(val);
                }
            }
            v1 = (float)System.Convert.ToDouble(calc.Pop());
            return v1;
        }
            
    }
}
