using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminiscenceBot.Modules
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HelpAttribute : Attribute
    {
        public string Help { get; }

        /// <summary>
        /// Attribute used to explain in-depth how the command can be used.
        /// </summary>
        /// <param name="help">The description that will be shown when help is requested for this command</param>
        public HelpAttribute(string help)
        {
            Help = help;
        }
    }
}
