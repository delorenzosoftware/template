using System;
using System.Linq;
using UnityEngine;

namespace Bazinga.Extensions
{
    public class ConditionalInactiveFieldAttribute : ConditionalFieldAttribute
    {
        public ConditionalInactiveFieldAttribute() : base("") { }

        /// <param name="fieldToCheck">String name of field to check value</param>
        /// <param name="inverse">Inverse check result</param>
        /// <param name="compareValues">On which values field will be shown in inspector</param>
        public ConditionalInactiveFieldAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues)
            : base(fieldToCheck, inverse, compareValues) { }
    }
}
