using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Linq;
using System.Resources;
using MegaZord.Library.Helpers;


namespace MegaZord.Library.DataAnnotation
{


    internal class MZClientDataTypeModelValidatorProvider : ClientDataTypeModelValidatorProvider
    {
        public MZClientDataTypeModelValidatorProvider()
        {

        }

        private static readonly HashSet<Type> _numericTypes = new HashSet<Type>(new Type[]
        {
            typeof(byte), typeof(sbyte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long?),
            typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
        });

        private static string _resourceClassKey;

        public static string ResourceClassKey
        {
            get { return _resourceClassKey ?? String.Empty; }
            set { _resourceClassKey = value; }
        }

        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return GetValidatorsImpl(metadata, context);
        }

        private static IEnumerable<ModelValidator> GetValidatorsImpl(ModelMetadata metadata, ControllerContext context)
        {
            Type type = metadata.ModelType;

            if (IsDateTimeType(type, metadata))
            {
                yield return new DateModelValidator(metadata, context);
            }

            if (IsNumericType(type))
            {
                yield return new NumericModelValidator(metadata, context);
            }
        }

        private static bool IsNumericType(Type type)
        {
            return _numericTypes.Contains(GetTypeToValidate(type));
        }

        private static bool IsDateTimeType(Type type, ModelMetadata metadata)
        {
            return typeof(DateTime) == GetTypeToValidate(type)
                && !String.Equals(metadata.DataTypeName, "Time", StringComparison.OrdinalIgnoreCase);
        }

        private static Type GetTypeToValidate(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type; // strip off the Nullable<>
        }

        // If the user specified a ResourceClassKey try to load the resource they specified.
        // If the class key is invalid, an exception will be thrown.
        // If the class key is valid but the resource is not found, it returns null, in which
        // case it will fall back to the MVC default error message.
        private static string GetUserResourceString(ControllerContext controllerContext, string resourceName)
        {
            string result = null;

            if (!String.IsNullOrEmpty(ResourceClassKey) && (controllerContext != null) && (controllerContext.HttpContext != null))
            {
                result = controllerContext.HttpContext.GetGlobalResourceObject(ResourceClassKey, resourceName, CultureInfo.CurrentUICulture) as string;
            }

            return result;
        }

        private static string GetFieldMustBeNumericResource(ControllerContext controllerContext)
        {
            return GetUserResourceString(controllerContext, "FieldMustBeNumeric") ?? MZHelperValidators.GetMensagemCampoPrecisaSerUmNumero("{0}");
        }

        private static string GetFieldMustBeDateResource(ControllerContext controllerContext)
        {
            return GetUserResourceString(controllerContext, "FieldMustBeDate") ?? MZHelperValidators.GetMensagemCampoPrecisaSerUmaData("{0}");
        }

        internal class ClientModelValidator : ModelValidator
        {
            private string _errorMessage;
            private string _validationType;

            public ClientModelValidator(ModelMetadata metadata, ControllerContext controllerContext, string validationType, string errorMessage)
                : base(metadata, controllerContext)
            {
                if (String.IsNullOrEmpty(validationType))
                {
                    throw new ArgumentException("Nulo", "validationType");
                }

                if (String.IsNullOrEmpty(errorMessage))
                {
                    throw new ArgumentException("Nulo", "errorMessage");
                }

                _validationType = validationType;
                _errorMessage = errorMessage;
            }

            public sealed override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
            {
                ModelClientValidationRule rule = new ModelClientValidationRule()
                {
                    ValidationType = _validationType,
                    ErrorMessage = FormatErrorMessage(Metadata.GetDisplayName())
                };

                return new ModelClientValidationRule[] { rule };
            }

            private string FormatErrorMessage(string displayName)
            {
                // use CurrentCulture since this message is intended for the site visitor
                return String.Format(CultureInfo.CurrentCulture, _errorMessage, displayName);
            }

            public sealed override IEnumerable<ModelValidationResult> Validate(object container)
            {
                // this is not a server-side validator
                return Enumerable.Empty<ModelValidationResult>();
            }
        }

        internal sealed class DateModelValidator : ClientModelValidator
        {
            public DateModelValidator(ModelMetadata metadata, ControllerContext controllerContext)
                : base(metadata, controllerContext, "date", GetFieldMustBeDateResource(controllerContext))
            {
            }
        }

        internal sealed class NumericModelValidator : ClientModelValidator
        {
            public NumericModelValidator(ModelMetadata metadata, ControllerContext controllerContext)
                : base(metadata, controllerContext, "number", GetFieldMustBeNumericResource(controllerContext))
            {
            }
        }
    }

    public class MZDataAnnotations
    {
        public static void Register()
        {
            var existingProvider = ModelValidatorProviders.Providers.Single(x => x is ClientDataTypeModelValidatorProvider);
            if (existingProvider != null)
                ModelValidatorProviders.Providers.Remove(existingProvider);
            ModelValidatorProviders.Providers.Add(new MZClientDataTypeModelValidatorProvider());
        }

    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class MZEmailAttribute : DataTypeAttribute, IClientValidatable
    {
        private readonly Regex _regex;
        private readonly string _field;

        public MZEmailAttribute(string field)
            : base(DataType.EmailAddress)
        {
            _field = field;
            _regex = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
            ErrorMessage = MZHelperValidators.GetMensagemCampoPrecisaSerUmEmail(_field);
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            var input = value as string;
            return ((input != null) && (_regex.Match(input).Length > 0));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,
                ValidationType = "email"
            };

            yield return rule;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class MZLengthAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly int _min;
        private readonly int _max;
        private readonly string _field;
        public MZLengthAttribute(int minlength, int maxlength, string field)
        {
            this._min = minlength;
            this._max = maxlength;
            this._field = field;
            this.ErrorMessage = MZHelperValidators.GetMensagemTamanhoCampo(field, minlength, maxlength);

        }


        private void EnsureLegalLengths()
        {

            if (this._max < this._min)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Valores inválidos para o tamanho, máxmio de {0} é menor que o mínimo de {1} caracteres.", new object[] { this._max, this._min }));
            }
        }


        public override string FormatErrorMessage(string name)
        {
            this.EnsureLegalLengths();
            return ErrorMessage;
        }

        public override bool IsValid(object value)
        {
            this.EnsureLegalLengths();
            int num = (value == null) ? 0 : Convert.ToString(value).Length;
            return ((value == null) || ((num >= this._min) && (num <= this._max)));
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            this.EnsureLegalLengths();
            var rule = new ModelClientValidationStringLengthRule(this.ErrorMessage, this._min,
                                                                 this._max)
                {
                    ValidationType = "length"
                };

            yield return rule;
        }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class MZRequiredAttribute : RequiredAttribute, IClientValidatable
    {

        public MZRequiredAttribute(string field)
        {
            this.AllowEmptyStrings = false;
            this.ErrorMessage = MZHelperValidators.GetMensagemRequerido(field);
        }



        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule
            {
                ErrorMessage = this.ErrorMessage,

                ValidationType = "required"
            };

            yield return rule;
        }
    }


    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class MZRangeAttribute : ValidationAttribute, IClientValidatable
    {
        private object Minimum;
        private object Maximum;
        private Type OperandType;
        private MZRangeAttribute(string field, Type type, object minimum, object maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.OperandType = type;
            this.ErrorMessage = MZHelperValidators.GetMensagemIntervaloValor(field, minimum, maximum);

        }

        public MZRangeAttribute(double minimum, double maximum, string field)
            : this(field, typeof(double), minimum, maximum)
        {


        }


        public MZRangeAttribute(int minimum, int maximum, string field)
            : this(field, typeof(int), minimum, maximum)
        {

        }

        public override string FormatErrorMessage(string name)
        {
            this.SetupConversion();
            return this.ErrorMessage;
        }

        private void Initialize(IComparable minimum, IComparable maximum, Func<object, object> conversion)
        {
            if (minimum.CompareTo(maximum) > 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "O valor máximo do range {0} precisa ser maior que o valor mínimo {1}.", new object[] { maximum, minimum }));
            }
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.Conversion = conversion;
        }

        public override bool IsValid(object value)
        {
            this.SetupConversion();
            if (value == null)
            {
                return true;
            }
            var str = value as string;
            if ((str != null) && string.IsNullOrEmpty(str))
            {
                return true;
            }
            object obj2 = null;
            try
            {
                obj2 = this.Conversion(value);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
            var minimum = (IComparable)this.Minimum;
            var maximum = (IComparable)this.Maximum;
            return ((minimum.CompareTo(obj2) <= 0) && (maximum.CompareTo(obj2) >= 0));
        }

        private void SetupConversion()
        {
            if (this.Conversion == null)
            {
                object minimum = this.Minimum;
                object maximum = this.Maximum;
                if ((minimum == null) || (maximum == null))
                {
                    throw new InvalidOperationException("Precisa ser ajusatado um valor máximo ou mínimo.");
                }
                var type = minimum.GetType();
                if (type == typeof(int))
                {
                    this.Initialize((int)minimum, (int)maximum, v => Convert.ToInt32(v, CultureInfo.InvariantCulture));
                }
                else if (type == typeof(double))
                {
                    this.Initialize((double)minimum, (double)maximum, v => Convert.ToDouble(v, CultureInfo.InvariantCulture));
                }
                else
                {
                    type = this.OperandType;
                    if (type == null)
                    {
                        throw new InvalidOperationException("Não foi possível identificar o tipo de informação a ser comparada.");
                    }
                    var type2 = typeof(IComparable);
                    if (!type2.IsAssignableFrom(type))
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Não é possível comparar os tipos {0} e {1}.", new object[] { type.FullName, type2.FullName }));
                    }
                    var converter = TypeDescriptor.GetConverter(type);
                    var comparable = (IComparable)converter.ConvertFromString((string)minimum);
                    var comparable2 = (IComparable)converter.ConvertFromString((string)maximum);
                    Func<object, object> conversion = delegate(object value)
                    {
                        if ((value != null) && (value.GetType() == type))
                        {
                            return value;
                        }
                        return converter.ConvertFrom(value);
                    };
                    this.Initialize(comparable, comparable2, conversion);
                }
            }
        }

        private Func<object, object> Conversion { get; set; }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRangeRule(this.ErrorMessage, this.Minimum, this.Maximum)
            {
                ValidationType = "range"
            };

            yield return rule;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property,
        AllowMultiple = false)]
    public class MZMinValueAttribute : MZRangeAttribute
    {
        public MZMinValueAttribute(string field, int minimun)
            : base(minimun, Int32.MaxValue, field)
        {
            SetErrorMessage(field, minimun);
        }

        public MZMinValueAttribute(string field, double minimun)
            : base(minimun, Double.MaxValue, field)
        {
            SetErrorMessage(field, minimun);
        }

        private void SetErrorMessage(string field, object value)
        {
            this.ErrorMessage = MZHelperValidators.GetMensagemMaiorOuIgual(field, value);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property,
    AllowMultiple = false)]
    public class MZMaxValueAttribute : MZRangeAttribute
    {
        public MZMaxValueAttribute(string field, int maximum)
            : base(Int32.MinValue, maximum, field)
        {
            SetErrorMessage(field, maximum);
        }

        public MZMaxValueAttribute(string field, double maximum)
            : base(Double.MinValue, maximum, field)
        {
            SetErrorMessage(field, maximum);
        }

        private void SetErrorMessage(string field, object value)
        {
            this.ErrorMessage = MZHelperValidators.GetMensagemMenorOuIgual(field, value);
        }
    }

    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property,
   AllowMultiple = false)]
    public class MZNameColumnAttribute : DisplayNameAttribute
    {
        public MZNameColumnAttribute(string nome)
            : base(nome)
        {
        }
    }





}
