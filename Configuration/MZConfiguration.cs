using System;
using System.Configuration;

namespace MegaZord.Library.Configuration
{
    public class MZConfiguration : ConfigurationSection
    {


        [ConfigurationProperty("EnableMinify", DefaultValue = "true", IsRequired = true)]
        public bool EnableMinify
        {
            get
            {
                return (bool)this["EnableMinify"];
            }
            set
            { this["EnableMinify"] = value; }
        }

        [ConfigurationProperty("CacheTime", DefaultValue = "60", IsRequired = true)]
        public long CacheTime
        {
            get
            {
                return (long)this["CacheTime"];
            }
            set
            { this["CacheTime"] = value; }
        }

        [ConfigurationProperty("PublicCryptoKey", DefaultValue = "MegaZord", IsRequired = true)]
        public string PublicCryptoKey
        {
            get
            {
                return (string)this["PublicCryptoKey"];
            }
            set
            { this["PublicCryptoKey"] = value; }
        }

        [ConfigurationProperty("LengthPassowrdsAutomatic", DefaultValue = "8", IsRequired = true)]
        public long LengthPassowrdsAutomatic
        {
            get
            {
                return (long)this["LengthPassowrdsAutomatic"];
            }
            set
            { this["LengthPassowrdsAutomatic"] = value; }
        }



        [ConfigurationProperty("AppName", DefaultValue = "MegaZord", IsRequired = true)]
        public string AppName
        {
            get
            {
                return (string)this["AppName"];
            }
            set
            { this["AppName"] = value; }
        }
        [ConfigurationProperty("AppUrl", DefaultValue = "MegaZord", IsRequired = true)]
        public string AppUrl
        {
            get
            {
                return (string)this["AppUrl"];
            }
            set
            { this["AppUrl"] = value; }
        }

        [ConfigurationProperty("Email")]
        public MZEmailElement Email
        {
            get
            {
                return (MZEmailElement)this["Email"];
            }
            set
            { this["Email"] = value; }
        }

        [ConfigurationProperty("SocialNetwork")]
        public MZSocialNetworkElement SocialNetwork
        {
            get
            {
                return (MZSocialNetworkElement)this["SocialNetwork"];
            }
            set
            { this["SocialNetwork"] = value; }
        }

        [ConfigurationProperty("Log")]
        public MZLogElement Log
        {
            get
            {
                return (MZLogElement)this["Log"];
            }
            set
            { this["Log"] = value; }
        }

    }


    public class MZLogElement : ConfigurationElement
    {
        [ConfigurationProperty("FileSize", DefaultValue = "500000", IsRequired = true)]
        public long FileSize
        {
            get
            {
                return (long)this["FileSize"];
            }
            set
            {
                this["FileSize"] = value;
            }
        }
        [ConfigurationProperty("LogDebug", DefaultValue = "false", IsRequired = true)]
        public bool LogDebug
        {
            get
            {
                return (bool)this["LogDebug"];
            }
            set
            {
                this["LogDebug"] = value;
            }
        }

        [ConfigurationProperty("LogError", DefaultValue = "true", IsRequired = true)]
        public bool LogError
        {
            get
            {
                return (bool)this["LogError"];
            }
            set
            {
                this["LogError"] = value;
            }
        }

        [ConfigurationProperty("LogSQL", DefaultValue = "false", IsRequired = true)]
        public bool LogSQL
        {
            get
            {
                return (bool)this["LogSQL"];
            }
            set
            {
                this["LogSQL"] = value;
            }
        }


        [ConfigurationProperty("LogAudit", DefaultValue = "true", IsRequired = true)]
        public bool LogAudit
        {
            get
            {
                return (bool)this["LogAudit"];
            }
            set
            {
                this["LogAudit"] = value;
            }
        }




    }

    public class MZEmailElement : ConfigurationElement
    {


        [ConfigurationProperty("DefaultReceiver", DefaultValue = "", IsRequired = true)]
        public String DefaultReceiver
        {
            get
            {
                return (String)this["DefaultReceiver"];
            }
            set
            {
                this["DefaultReceiver"] = value;
            }
        }

        [ConfigurationProperty("DefaultDisplayName", DefaultValue = "", IsRequired = true)]
        public String DefaultDisplayName
        {
            get
            {
                return (String)this["DefaultDisplayName"];
            }
            set
            {
                this["DefaultDisplayName"] = value;
            }
        }

        [ConfigurationProperty("Receive")]
        public MZServerMailConfiguration Receive
        {
            get
            {
                return (MZServerMailConfiguration)this["Receive"];
            }
            set
            { this["Receive"] = value; }
        }


        [ConfigurationProperty("SpamSend")]
        public MZServerMailConfiguration SpamSend
        {
            get
            {
                return (MZServerMailConfiguration)this["SpamSend"];
            }
            set
            { this["SpamSend"] = value; }
        }

        [ConfigurationProperty("NormalSend")]
        public MZServerMailConfiguration NormalSend
        {
            get
            {
                return (MZServerMailConfiguration)this["NormalSend"];
            }
            set
            { this["NormalSend"] = value; }
        }
    }


    public class MZServerMailConfiguration : ConfigurationElement
    {

        [ConfigurationProperty("Server", DefaultValue = " ", IsRequired = true)]
        public String Server
        {
            get
            {
                return (String)this["Server"];
            }
            set
            {
                this["Server"] = value;
            }
        }

        [ConfigurationProperty("UserName", DefaultValue = "", IsRequired = true)]
        public String UserName
        {
            get
            {
                return (String)this["UserName"];
            }
            set
            {
                this["UserName"] = value;
            }
        }

        [ConfigurationProperty("Password", DefaultValue = "", IsRequired = true)]
        public String Password
        {
            get
            {
                return (String)this["Password"];
            }
            set
            {
                this["Password"] = value;
            }
        }


        [ConfigurationProperty("From", DefaultValue = "", IsRequired = true)]
        public String From
        {
            get
            {
                return (String)this["From"];
            }
            set
            {
                this["From"] = value;
            }
        }



        [ConfigurationProperty("EnableSsl", DefaultValue = true, IsRequired = true)]
        public bool EnableSsl
        {
            get
            {
                return (bool)this["EnableSsl"];
            }
            set
            {
                this["EnableSsl"] = value;
            }
        }



        [ConfigurationProperty("Port", DefaultValue = "0", IsRequired = true)]
        public int Port
        {
            get
            {
                return (int)this["Port"];
            }
            set
            {
                this["Port"] = value;
            }
        }

    }

    public class MZSocialNetworkElement : ConfigurationElement
    {
        [ConfigurationProperty("Facebook")]
        public MZFacebookElement Facebook
        {
            get
            {
                return (MZFacebookElement)this["Facebook"];
            }
            set
            { this["Facebook"] = value; }
        }

        [ConfigurationProperty("Twitter")]
        public MZTwitterElement Twitter
        {
            get
            {
                return (MZTwitterElement)this["Twitter"];
            }
            set
            { this["Twitter"] = value; }
        }


        [ConfigurationProperty("GooglePlus")]
        public MZGooglePlusElement GooglePlus
        {
            get
            {
                return (MZGooglePlusElement)this["GooglePlus"];
            }
            set
            { this["GooglePlus"] = value; }
        }
    }


    public class MZBaseSocialNetworkElement : ConfigurationElement
    {

        [ConfigurationProperty("Id", DefaultValue = " ", IsRequired = true)]
        public String Id
        {
            get
            {
                return (String)this["Id"];
            }
            set
            {
                this["Id"] = value;
            }
        }

        [ConfigurationProperty("Page", DefaultValue = " ", IsRequired = true)]
        public String Page
        {
            get
            {
                return (String)this["Page"];
            }
            set
            {
                this["Page"] = value;
            }
        }
    }

    public class MZTwitterElement : MZBaseSocialNetworkElement
    {
        [ConfigurationProperty("DataText", DefaultValue = " ", IsRequired = true)]
        public String DataText
        {
            get
            {
                return (String)this["DataText"];
            }
            set
            {
                this["DataText"] = value;
            }
        }

        [ConfigurationProperty("DataHashTags", DefaultValue = " ", IsRequired = true)]
        public String DataHashTags
        {
            get
            {
                return (String)this["DataHashTags"];
            }
            set
            {
                this["DataHashTags"] = value;
            }
        }
    }

    public class MZGooglePlusElement : MZBaseSocialNetworkElement
    {
    }

    public class MZFacebookElement : MZBaseSocialNetworkElement
    {
       

        [ConfigurationProperty("AccessToken", DefaultValue = " ", IsRequired = true)]
        public String AccessToken
        {
            get
            {
                return (String)this["AccessToken"];
            }
            set
            {
                this["AccessToken"] = value;
            }
        }

        [ConfigurationProperty("AppId", DefaultValue = " ", IsRequired = true)]
        public String AppId
        {
            get
            {
                return (String)this["AppId"];
            }
            set
            {
                this["AppId"] = value;
            }
        }

        
    }
}
