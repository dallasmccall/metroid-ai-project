using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace CS8803AGA.world.space
{
    abstract class AMissionTerminalExpander : IMissionTerminalExpander
    {
        public String TerminalName { get; set; }
        public String MissionNodeID { get; set; }
        public ParametersTable Parameters { get; protected set; }

        protected static int s_instanceIDCounter = 0;
        protected int m_instanceID;

        protected List<String> m_linearRequirements = new List<String>();

        protected IFractalCreator m_fractalCreator;

        protected static readonly ParametersTable s_defaultParams = new ParametersTable
        {
            {"FractalCreator", "FractalCreatorJagged"},
        };

        protected AMissionTerminalExpander(
            String terminalName,
            String missionNodeID,
            ParametersTable subclassDefaultParameters,
            ParametersTable terminalParameters)
        {
            m_instanceID = s_instanceIDCounter;
            s_instanceIDCounter++;

            TerminalName = terminalName;
            MissionNodeID = missionNodeID;
            Parameters = new ParametersTable();
            foreach (KeyValuePair<String, String> kvp in s_defaultParams)
            {
                Parameters.Add(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<String, String> kvp in subclassDefaultParameters)
            {
                Parameters.Add(kvp.Key, kvp.Value);
            }

            SetParameters(terminalParameters);
        }

        protected abstract void InitializeMyParams();

        #region IMissionTerminalExpander Members

        public virtual void AddLinearRequirement(string missionID)
        {
            m_linearRequirements.Add(missionID);
        }

        public virtual ICollection<string> LinearReqs
        {
            get { return m_linearRequirements; }
        }

        public virtual bool PassesMissionRequirements(ISpace space, Point markedPoint, Point unmarkedPoint)
        {
            foreach (String linearReq in m_linearRequirements)
            {
                if (!space.GetQualifiers(markedPoint).Contains(linearReq))
                {
                    return false;
                }
            }
            return true;
        }

        public abstract float Evaluate(ISpace space, Point markedPoint, Point unmarkedPoint);

        public abstract void Expand(ISpace space, Point markedPoint, Point unmarkedPoint);

        public virtual void AddParameter(String key, String value)
        {
            Parameters[key] = value;
        }

        protected virtual void SetParameters(ParametersTable parameters)
        {
            foreach (KeyValuePair<String, String> kvp in parameters)
            {
                //if (!Parameters.ContainsKey(kvp.Key))
                //{
                //    throw new Exception(String.Format("Unknown parameter key '{0}' passed to '{1}'", kvp.Key, this.GetType()));
                //}
                Parameters[kvp.Key] = kvp.Value;
            }
        }

        public override int GetHashCode()
        {
            return m_instanceID.GetHashCode();
        }

        #endregion

        public void InitializeParams()
        {
            m_fractalCreator = Parameters.ParseFractalCreator("FractalCreator");
            InitializeMyParams();
        }
    }
}
