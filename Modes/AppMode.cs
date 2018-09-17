using System;
using System.Collections.Generic;

namespace M3uParser.Modes
{
    public abstract class AppMode
    {
        private Dictionary<string, string> _appParams = new Dictionary<string, string>();

        protected AppMode(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var isParam = args[i].StartsWith('-');
                var hasValue = i < args.Length - 1 && !args[i + 1].StartsWith('-');

                if (isParam)
                    this._appParams.Add(args[i].Substring(1), hasValue ? args[i + 1] : null);
            }

        }

        protected string GetParamValueString(string paramName)
        {
            return this._appParams.ContainsKey(paramName) ? this._appParams[paramName] : null;
        }

        protected int? GetParamValueInt(string paramName)
        {
            if (!this._appParams.ContainsKey(paramName))
                return null;
            
            var stringValue = GetParamValueString(paramName);
            if (!string.IsNullOrEmpty(stringValue))
                return int.Parse(stringValue);
            else
                return null;
        }


        public abstract void Run();
    }
}
