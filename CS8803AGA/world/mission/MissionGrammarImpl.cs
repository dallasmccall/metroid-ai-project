using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using MetroidAI.world.space;
using MetroidAIGameLibrary.player;

namespace MetroidAI.world.mission
{
    class MissionGrammarImpl : IMissionGrammar
    {
        protected Dictionary<String, GraphGrammarRule> Rules;

        protected List<String> AvailableRules;

        protected Dictionary<String, TerminalCreationInfo> Terminals;

        #region Construction

        public MissionGrammarImpl(Stream stream)
        {
            Rules = new Dictionary<string,GraphGrammarRule>();
            AvailableRules = new List<string>();

            XmlTextReader reader = new XmlTextReader(stream);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            XmlNode grammarNode = doc.FirstChild;
            XmlNodeList ruleNodes = grammarNode.SelectNodes("GraphGrammarRule");
            foreach (XmlNode ruleNode in ruleNodes)
            {
                GraphGrammarRule rule = new GraphGrammarRule(ruleNode);
                this.Rules.Add(rule.Name, rule);
                //If there are no requirements for this rule, then it is a valid rule to start with
                if (rule.isRoot)
                {
                    this.AvailableRules.Add(rule.Name);
                    Console.WriteLine("Available: " + rule.Name);
                }
            }

            Terminals = new Dictionary<String, TerminalCreationInfo>();

            Console.WriteLine("Terminals:");
            XmlNode terminals = grammarNode.SelectSingleNode("Terminals");
            XmlNodeList terminalNodes = terminals.SelectNodes("Terminal");

            foreach (XmlNode terminalNode in terminalNodes)
            {
                ParametersTable parameters = new ParametersTable();

                XmlNode nameNode = terminalNode.SelectSingleNode("Name");
                XmlNode classNode = terminalNode.SelectSingleNode("Class");

                Console.WriteLine("\t" + nameNode.InnerText);
                Console.WriteLine("\t\t" + classNode.InnerText);

                XmlNode paramsNode = terminalNode.SelectSingleNode("Params");
                if (paramsNode != null)
                {
                    XmlNodeList paramNodes = paramsNode.SelectNodes("Param");
                    foreach (XmlNode paramNode in paramNodes)
                    {
                        XmlNode keyNode = paramNode.SelectSingleNode("Key");
                        XmlNode valueNode = paramNode.SelectSingleNode("Value");
                        Console.WriteLine(String.Format("\t\t{0}:{1}", keyNode.InnerText, valueNode.InnerText));
                        parameters.Add(keyNode.InnerText, valueNode.InnerText);
                    }
                }

                TerminalCreationInfo tci =
                    new TerminalCreationInfo(nameNode.InnerText, classNode.InnerText, parameters);
                Terminals.Add(nameNode.InnerText, tci);
            }
        }

        protected class TerminalCreationInfo
        {
            public String Name { get; protected set; }
            public String Class { get; protected set; }
            public ParametersTable Parameters { get; protected set; }

            public TerminalCreationInfo(String name, String className, ParametersTable parameters)
            {
                Name = name;
                Class = className;
                Parameters = parameters;
            }

            public IMissionTerminalExpander Create(string missionNodeID)
            {
                Type[] doubleStringCtorParams =
                    new Type[] { typeof(String), typeof(String), typeof(ParametersTable) };
                /*FIXME - Totally ugly hack! -Dallas 19/3/2011*/
                Type typeOfTargetClass = Type.GetType(
                    String.Format("{0}.{1}", "MetroidAI.world.space.expanders", Class),
                    true,
                    true);
                ConstructorInfo ctor = typeOfTargetClass.GetConstructor(doubleStringCtorParams);

                if (ctor == null)
                {
                    throw new Exception(
                        String.Format("Expander type '{0}' does not support a two-string, dict<str,str> ctor",
                            Class));
                }

                object[] args = new object[] { Name, missionNodeID, Parameters };
                IMissionTerminalExpander mte = (IMissionTerminalExpander)ctor.Invoke(args);
                return mte;
            }
        }

        #endregion

        #region Static Methods

        public static MissionGrammarImpl LoadFromFile(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            return new MissionGrammarImpl(fs);
        }

        #endregion
       
        public IMission WalkGrammar(MetroidAIGameLibrary.player.PlayerModel playerModel)
        {
            /* TODO:
             *  Add Weights to possible paths to take into account the player model
             *  This means I'll have to sort the list of available rules to be best first
             *  Implement a vaild walk of the grammar (basically, everything)
             */
            // start with an empty mission graph, then build it up
            MissionImpl mission = new MissionImpl();

            while (AvailableRules.Count > 0)
            {
                Console.WriteLine("Expanding: " + AvailableRules[0]);
                ExecuteRule(mission, AvailableRules[0]);
                AvailableRules.RemoveAt(0);
            }
            Console.WriteLine("Printing Nodes:");
            foreach (MissionNode node in mission.MissionNodes)
            {
                node.printNode();
            }
            Console.WriteLine("Done Printing Nodes");

            mission.PostProcess();

            return mission;
        }

        private void ExecuteRule(MissionImpl mission, String rule)
        {
            GraphGrammarRule grammarRule = Rules[rule];
            MissionNode nodeToReplace = null;

            if (mission.MissionNodes.Count == 0)
            {
                MissionNode tempNode = new MissionNode(rule);
                mission.MissionNodes.Add(tempNode);
            }

            foreach (MissionNode node in mission.MissionNodes)
            {
                if (node.Name.Equals(rule))
                {
                    Console.WriteLine("Found Node to Replace: " + node.Name);
                    nodeToReplace = node;
                    break;
                }
            }

            List<MissionNode> missionNodes = new List<MissionNode>();
            for (int i = 0; i < grammarRule.Expansions.Count; i++)
            {
                GraphGrammarRule.Expansion expansion = grammarRule.Expansions[i];
                if (expansion.Probability > RandomManager.get().NextDouble() || i == grammarRule.Expansions.Count - 1)
                {
                    // Use this expansion

                    foreach (String param in expansion.Params)
                    {
                        ParamContainer pc = new ParamContainer(param);
                        nodeToReplace.ParamContainers.Add(pc);
                        mission.ParamContainers.Add(pc);
                    }

                    foreach (KeyValuePair<String, String> mapping in expansion.ResultMappings)
                    {
                        MissionNode node = new MissionNode(mapping.Value, nodeToReplace.ParamContainers);
                        node.expansionID = mapping.Key;

                        Console.WriteLine("Node to add in: " + node.Name);

                        if (!Terminals.ContainsKey(node.Name))
                        {
                            AvailableRules.Add(node.Name);
                            Console.WriteLine("Adding to Available: " + node.Name);
                        }
                        else
                        {
                            node.Expander = Terminals[node.Name].Create(node.ID.ToString());
                        }

                        missionNodes.Add(node);
                    }

                    foreach (KeyValuePair<KeyValuePair<String, String>, MissionEdge> edge in expansion.ResultEdges) 
                    {
                        String from = edge.Key.Key;
                        String to = edge.Key.Value;

                        MissionNode fromNode = null;
                        MissionNode toNode = null;

                        foreach (MissionNode node in missionNodes)
                        {
                            if (node.expansionID.Equals(from))
                            {
                                fromNode = node;
                            }
                            if (node.expansionID.Equals(to))
                            {
                                toNode = node;
                            }
                        }
                        MissionConnection tempConnection =
                            new MissionConnection(fromNode.ID, toNode.ID, edge.Value);

                        fromNode.Connections.Add(tempConnection);
                        toNode.Connections.Add(tempConnection);
                    }

                    MissionNode inNode = null;
                    MissionNode outNode = null;

                    foreach (MissionNode node in missionNodes)
                    {
                        if (node.expansionID.Equals(expansion.InNode))
                        {
                            inNode = node;
                        }
                        if (node.expansionID.Equals(expansion.OutNode))
                        {
                            outNode = node;
                        }
                    }

                    foreach (MissionNode node in mission.MissionNodes)
                    {
                        foreach (MissionConnection conn in node.Connections)
                        {
                            if (conn.DestID == nodeToReplace.ID)
                            {
                                conn.DestID = inNode.ID;
                                inNode.Connections.Add(conn);
                            }
                            if (conn.SourceID == nodeToReplace.ID)
                            {
                                conn.SourceID = outNode.ID;
                                outNode.Connections.Add(conn);
                            }
                        }
                    }

                    foreach (MissionConnection conn in nodeToReplace.Connections)
                    {
                        if (conn.DestID == nodeToReplace.ID)
                        {
                            conn.DestID = inNode.ID;
                            inNode.Connections.Add(conn);
                        }
                        if (conn.SourceID == nodeToReplace.ID)
                        {
                            conn.SourceID = outNode.ID;
                            outNode.Connections.Add(conn);
                        }
                    }
                    foreach (MissionNode node in missionNodes)
                    {
                        mission.MissionNodes.Add(node);
                    }
                    mission.MissionNodes.Remove(nodeToReplace);
                    nodeToReplace.freeID();
                    return;
                }
            }
        }
    }

    class GraphGrammarRule
    {
        public struct Expansion
        {
            public double Probability;

            public String InNode, OutNode;

            public List<String> Params;

            public List<KeyValuePair<String, String>> ResultMappings;

            public List<KeyValuePair<KeyValuePair<String, String>, MissionEdge>> ResultEdges;
        }

        public double getProbability(String prob)
        {
            PlayerModel tempModel = new PlayerModel();
            if (prob.Equals("Explore"))
            {
                if (Settings.getInstance().IsExplorer)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            if (prob.Equals("Kill"))
            {
                if (Settings.getInstance().IsExplorer)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            //TODO get probability from player model
            return Convert.ToDouble(prob);
        }

        public List<Expansion> Expansions { get; protected set; }

        public String Name {get; protected set; }

        public bool isRoot { get; protected set; }

        public GraphGrammarRule(XmlNode ruleNode)
        {
            #region Get Information about the Rule
            this.Name = ((XmlElement)ruleNode).GetAttribute("Name");

            Console.WriteLine("Rule: " + Name);

            this.isRoot = Convert.ToBoolean(ruleNode.SelectSingleNode("Root").InnerText);

            Console.WriteLine("Is Root: " + this.isRoot);
            #endregion

            #region Get Results of Expansion
            this.Expansions = new List<Expansion>();

            XmlNodeList ExpansionsList = ruleNode.SelectNodes("Expansion");

            foreach (XmlNode Expansion in ExpansionsList)
            {
                Expansion tempExpansion = new Expansion();

                tempExpansion.Probability = this.getProbability(((XmlElement)Expansion).GetAttribute("Prob"));

                Console.WriteLine("Probability: " + tempExpansion.Probability);


                tempExpansion.Params = new List<string>();
                XmlNode paramsNode = Expansion.SelectSingleNode("Params");
                if (paramsNode != null)
                {
                    foreach (XmlNode paramNode in paramsNode.SelectNodes("Param"))
                    {
                        tempExpansion.Params.Add(paramNode.InnerText);

                        Console.WriteLine("\tParam " + paramNode.InnerText);
                    }
                }

                XmlNode r_mappings = Expansion.SelectSingleNode("ResultMappings");
                XmlNodeList r_mappingsList = r_mappings.SelectNodes("Node");
                Console.WriteLine("\tResult Mapping");

                tempExpansion.ResultMappings = new List<KeyValuePair<string, string>>();
                foreach (XmlNode r_mapping in r_mappingsList)
                {
                    String ID = ((XmlElement)r_mapping).GetAttribute("ID");
                    String Element = r_mapping.InnerText;
                    Console.WriteLine("\t\tID: " + ID + " Element: " + Element);
                    KeyValuePair<String, String> pair = new KeyValuePair<String, String>(ID, Element);
                    tempExpansion.ResultMappings.Add(pair);
                }

                tempExpansion.InNode = r_mappings.SelectSingleNode("InNode").InnerText;
                tempExpansion.OutNode = r_mappings.SelectSingleNode("OutNode").InnerText;

                XmlNode r_edges = Expansion.SelectSingleNode("ResultEdges");
                XmlNodeList r_edgesList = r_edges.SelectNodes("Edge");
                Console.WriteLine("\tResult Edges:");

                tempExpansion.ResultEdges = new List<KeyValuePair<KeyValuePair<string, string>, MissionEdge>>();
                foreach (XmlNode r_edge in r_edgesList)
                {
                    MissionEdge edgeType;

                    String from = r_edge.SelectSingleNode("From").InnerText;
                    String to = r_edge.SelectSingleNode("To").InnerText;
                    String type = r_edge.SelectSingleNode("Type").InnerText;

                    Console.WriteLine("\t\t(" + from + "," + to + ") Type: " + type);
                    if (type.Equals("Linear"))
                    {
                        edgeType = MissionEdge.Linear;
                    }
                    else if (type.Equals("Parallel"))
                    {
                        edgeType = MissionEdge.Parallel;
                    }
                    else
                    {
                        //Default, shouldn't happen...
                        edgeType = MissionEdge.Linear;
                    }

                    KeyValuePair<String, String> edgePair = new KeyValuePair<string, string>(from, to);
                    KeyValuePair<KeyValuePair<String, String>, MissionEdge> resultEdgePair = new KeyValuePair<KeyValuePair<string, string>, MissionEdge>(edgePair, edgeType);
                    tempExpansion.ResultEdges.Add(resultEdgePair);
                }
                this.Expansions.Add(tempExpansion);
            }
            

            #endregion
        }
    }


}
