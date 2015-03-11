// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Arguments.cs" company="Allberg Konsult AB">
//   Allberg Konsult AB
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace NetworkRelay
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The arguments.
    /// </summary>
    internal class Arguments
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Arguments"/> class.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        public Arguments(string[] args)
        {
            // First argument is the hostname.
            this.Hostname = args[0];

            // The rest is port numbers
            var ports = new List<int>();

            for (var i = 1; i < args.Length; i++)
            {
                int somePort;
                if (!int.TryParse(args[i], out somePort))
                {
                    throw new ApplicationException(string.Format("\"{0}\" has to be a number between 1 and 65535", args[i]));
                }

                ports.Add(somePort);
            }

            this.Ports = ports;
        }

        /// <summary>
        /// Gets the hostname.
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        /// Gets the ports.
        /// </summary>
        public IList<int> Ports { get; private set; }
    }
}
