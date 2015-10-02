using System;
using System.Collections.Generic;
//using MailChimp.Types;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Messaging.Events;
using Orchard.Messaging.Models;
using System.Linq;

namespace Cascade.WebShop.Handlers
{
    public class EmailHandler : IMessageEventHandler
    {


        public EmailHandler()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        /// <summary>
        /// Set up the Body and Subject of the email
        /// </summary>
        /// <param name="context"></param>
        public void Sending(MessageContext context) 
        {
            switch(context.Type)
            {
                case "ORDER_RECEIVED":
                    context.MailMessage.Body = context.Properties["Body"];
                    context.MailMessage.Subject = context.Properties["Subject"];
                    break;
                case "WELCOME":
                    context.MailMessage.Subject = context.Properties["Subject"];
                    context.MailMessage.Body = MergeBody(context.Properties);
                    break;
                case "SUBSCRIBE":
                    try
                    {
                        string apiKey = context.Properties["MailChimpApiKey"]; // "9fc63c6d93d5d5fdaa936e349006e1c2-us6";
                        string listName = context.Properties["MailChimpListName"]; // "Cascade Print Room Subscriber Mailing List";
                        string groupName = context.Properties["MailChimpGroupName"]; // "Snail Mail";
                        string groupValue = context.Properties["MailChimpGroupValue"]; // "Snail mail (please provide an address above)";

                        // add subscriber to our MailChimp list
                        var mc = new MailChimp.MCApi(apiKey, true);
                        var result = mc.Ping();
                        if (result != "Everything's Chimpy!")
                            throw new Exception("invalid api key");

                        // retrieve list id
                        var lists = mc.Lists();

                        if (lists == null || lists.Data == null || !lists.Data.Any())
                            throw new Exception("no lists");

                        var list = lists.Data.FirstOrDefault(l => l.Name == listName);
                        if (list == null)
                            throw new Exception(string.Format("unable to find a list called '{0}'", listName));

                        // build groups
                        var groupings = new List<MailChimp.Types.List.Grouping>();

                        if (context.Properties["ReceivePost"] == "True")
                        {
                            groupings.Add(new MailChimp.Types.List.Grouping(groupName,
                                                                            new string[] { groupValue })
                                         );
                        }


                        // options
                        //var options = new MailChimp.Types.List.SubscribeOptions { DoubleOptIn = false, EmailType = MailChimp.Types.List.EmailType.Html, UpdateExisting = true, ReplaceInterests = true };

                        // build merge vars
                        const string spaces = "  ";
                        var mergeVars = new MailChimp.Types.List.Merges(context.Addresses.First(),
                                                                        MailChimp.Types.List.EmailType.Html,
                                                                        groupings.ToArray())
                            {
                                {"FNAME", context.Properties["FirstName"]},
                                {"LNAME", context.Properties["LastName"]},
                                {"ADDRESS", context.Properties["Address"]
                                               + spaces + context.Properties["City"]
                                               + spaces + context.Properties["State"]
                                               + spaces + context.Properties["Postcode"]
                                               + spaces + context.Properties["CountryCode"]
                                }
                            };
                     

                        // subscribe
                        mc.ListSubscribe(list.ListID, context.Addresses.First(), mergeVars);

                    }
                    catch (Exception ex)
                    {
                        Logger.Error("MailChimp failed: {0}", ex.Message);
                    }
                    break;
            }
        }

        private string MergeBody(System.Collections.Generic.Dictionary<string, string> props)
        {
            //                                                       0                   1                  2
            string body = string.Format(props["BodyTemplate"], props["FirstName"], props["LastName"]);
            if (string.Compare(props["Subscribed"], "true", StringComparison.OrdinalIgnoreCase) == 0 && !string.IsNullOrWhiteSpace(props["UnsubscribeEmail"]))
                body += "<br/><br/>PS You are subscribed to our mailing list. To unsubscribe please send an email to " + props["UnsubscribeEmail"];
            return body;

        }

        public void Sent(MessageContext context)
        {
          
        }
    }
}
