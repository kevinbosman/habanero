using System;
using System.Collections.Generic;
using Habanero.Base;

namespace Habanero.Bo
{
    /// <summary>
    /// Checks integer values against property rules that test for validity
    /// </summary>
    public class PropRuleInteger : PropRuleBase
    {
        private int _minValue = int.MinValue;
        private int _maxValue = int.MaxValue;

        /// <summary>
        /// Constructor to initialise a new rule
        /// </summary>
        /// <param name="name">The rule name</param>
        /// <param name="message">This rule's failure message</param> 
        /// <param name="min">The minimum value allowed for the integer</param>
        /// <param name="max">The maximum value allowed for the integer</param>
        public PropRuleInteger(string name, string message, int min, int max)
			: base(name, message)
        {
            _minValue = min;
            _maxValue = max;
        }

        /// <summary>
        /// Constructor to initialise a new rule
        /// </summary>
        /// <param name="name">The rule name</param>
        /// <param name="message">This rule's failure message</param>
        /// <param name="parameters">The parameters for this rule.  Valid parameters are "min" and "max"</param>
        public PropRuleInteger(string name, string message, Dictionary<string, object> parameters)
			: base(name, message)
		{
			base.Parameters = parameters;
		}

        /// <summary>
        /// Sets up the parameters to the rule, that is the individual pairs
        /// of rule type and rule value that make up the composite rule
        /// </summary>
		protected internal override void SetupParameters()
		{
            try
            {
                foreach (string key in _parameters.Keys)
                {
                    object value = _parameters[key];
					if (value != null)
					{
						switch (key)
						{
							case "min":
								_minValue = Convert.ToInt32(value);
								break;
							case "max":
								_maxValue = Convert.ToInt32(value);
								break;
							default:
								throw new InvalidXmlDefinitionException(String.Format(
                                	"The rule type '{0}' for integers does not exist. " +
                                	"Check spelling and capitalisation, or see the " +
                                	"documentation for existing options or ways to " +
                                	"add options of your own.", key));
						}
					}
                }
            }
            catch (InvalidXmlDefinitionException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidXmlDefinitionException("An error occurred " +
                    "while processing the property rules for an integer.  The " +
                    "likely cause is that one of the attributes in the 'add' " +
                    "element of the class definitions has an invalid value.", ex);
            }
        }

        /// <summary>
        /// Gets and sets the minimum value that the integer can be assigned
        /// </summary>
        public int MinValue
        {
            get { return _minValue; }
        	protected set { _minValue = value; }
        }

        /// <summary>
        /// Gets and sets the maximum value that the integer can be assigned
        /// </summary>
        public int MaxValue
        {
            get { return _maxValue; }
        	protected set { _maxValue = value; }
        }

        /// <summary>
        /// Indicates whether the property value is valid against the rules
        /// </summary>
        /// <param name="propName">The property name being checked</param>
        /// <param name="propValue">The value to check</param>
        /// <param name="errorMessage">A string to amend with an error
        /// message indicating why the value might have been invalid</param>
        /// <returns>Returns true if valid</returns>
        protected internal override bool isPropValueValid(string propName, object propValue, ref string errorMessage)
        {
            bool valueValid = base.isPropValueValid(propName, propValue, ref errorMessage);
            if (propValue is int)
            {
                int intPropRule = (int)propValue;
                if (intPropRule < _minValue)
                {
                    errorMessage = String.Format("'{0}' is not valid for the rule '{1}'. ",
                        propName, Name);
                    if (Message != null) errorMessage += Message;
                    else errorMessage += "The value cannot be less than " + _minValue + ".";
                    valueValid = false;
                }
                if (intPropRule > _maxValue)
                {
                    errorMessage = String.Format("'{0}' is not valid for the rule '{1}'. ",
                        propName, Name);
                    if (Message != null) errorMessage += Message;
                    else errorMessage += "The value cannot be more than " + _maxValue + ".";
                    valueValid = false;
                }
            }
            return valueValid;
            
        }

        /// <summary>
        /// Returns the list of available parameter names for the rule.
        /// </summary>
        /// <returns>A list of the parameters that this rule uses</returns>
    	protected internal override List<string> AvailableParameters()
    	{
			List<string> parameters = new List<string>();
			parameters.Add("min");
			parameters.Add("max");
			return parameters;
    	}
    }
}