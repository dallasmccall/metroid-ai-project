using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CS8803AGA.world.space
{
    class ParametersTable : Dictionary<String, String>
    {
        public int ParseInt(string param)
        {
            if (this[param].Contains('-'))
            {
                String s = this[param];
                String left = s.Substring(0, s.IndexOf('-'));
                String right = s.Substring(s.IndexOf('-') + 1);
                return RandomManager.get().Next(Int32.Parse(left), Int32.Parse(right));
            }
            else
            {
                return Int32.Parse(this[param]);
            }
        }

        public Direction ParseDirection(string param)
        {
            String s = this[param];
            if (s == "Random")
            {
                int r = RandomManager.get().Next(4);
                switch (r)
                {
                    case 0: return Direction.Up;
                    case 1: return Direction.Left;
                    case 2: return Direction.Right;
                    case 3: return Direction.Down;
                    default: return Direction.Up;
                }
            }
            else if (s == "HRandom")
            {
                int r = RandomManager.get().Next(2);
                switch (r)
                {
                    case 0: return Direction.Right;
                    case 1: return Direction.Left;
                    default: return Direction.Left;
                }
            }
            else if (s == "VRandom")
            {
                int r = RandomManager.get().Next(2);
                switch (r)
                {
                    case 0: return Direction.Up;
                    case 1: return Direction.Down;
                    default: return Direction.Up;
                }
            }
            else
            {
                return Direction.ParseString(s);
            }
        }

        public T ParseObject<T>(String classPath, String paramName)
        {
            String className = this[paramName];
            String fullClassName = String.Format("{0}.{1}", classPath, className);
            object o;
            try
            {
                o = Assembly.GetCallingAssembly().CreateInstance(
                    fullClassName,
                    true);
            }
            catch
            {
                Type[] parametersArgs = new Type[] { typeof(ParametersTable) };
                Type type = Type.GetType(fullClassName, true, true);
                ConstructorInfo ctor = type.GetConstructor(parametersArgs);
                o = ctor.Invoke(new object[] { this });
            }

            return (T)o;
        }

        public IMissionTerminalExpander ParseMissionTerminalExpander(String paramName)
        {
            return ParseObject<IMissionTerminalExpander>("CS8803AGA.world.space.expanders", paramName);
        }

        public IFractalCreator ParseFractalCreator(String paramName)
        {
            return ParseObject<IFractalCreator>("CS8803AGA.world.space", paramName);
        }

        public IScreenPostProcessor ParseScreenPostProcessor(String paramName)
        {
            return ParseObject<IScreenPostProcessor>("CS8803AGA.world.space.postprocessors", paramName);
        }

        public IObjectPopulator ParseObjectPopulator(String paramName)
        {
            return ParseObject<IObjectPopulator>("CS8803AGA.world.space.populators", paramName);
        }

        public T ParseEnum<T>(String paramName)
        {
            return (T)Enum.Parse(typeof(T), this[paramName]);
        }

        public delegate T ParseFn<T>(String parameter);
        public List<T> ParseList<T>(String listName, ParseFn<T> parser)
        {
            List<T> lst = new List<T>();

            int idx = 0;
            String key = String.Format("{0}[{1}]", listName, idx);
            while (this.ContainsKey(key))
            {
                lst.Add(parser(key));
                idx++;
                key = String.Format("{0}[{1}]", listName, idx);
            }
            return lst;
        }
    }
}
