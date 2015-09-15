using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TRex.Metadata;
using SendGrid;
using SendGridAPI.Models;
using System.Net.Mail;
using System.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.AppService.ApiApps.Service;
using Swashbuckle.Swagger.Annotations;

namespace SendGridAPI.Controllers
{
    public class SendGridController : ApiController
    {
        public static Dictionary<string, Uri> CallbackStore = new Dictionary<string, Uri>();

        [HttpPost, Route("api/SendEmail")]
        [Metadata("Send Email", "Send an email via SendGrid")]
        
        public async Task<HttpResponseMessage> SendEmail(Email email)

        {
            var myMessage = new SendGridMessage();
            myMessage.From = new MailAddress(email.from);

            List<string> recipients = email.recipients.Split(',', ';').ToList<string>();

            myMessage.AddTo(recipients);
            myMessage.Subject = email.subject;
            myMessage.Text = string.IsNullOrEmpty(email.text) ? null : email.text;
            myMessage.Html = string.IsNullOrEmpty(email.html) ? null : email.html;
            myMessage.EnableClickTracking(email.clickTracking);

            var transportWeb = new Web(ConfigurationManager.AppSettings["SendgridApiKey"]);

            await transportWeb.DeliverAsync(myMessage);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost, Route("webhook")]
        [Metadata("SendGrid Webhook", null, VisibilityType.Internal)]
        public async Task<HttpResponseMessage> Webhook(JArray events)
        {
            foreach (var e in events)
            {
                foreach (var c in CallbackStore)
                {
                    var callback = new ClientTriggerCallback<JObject>(c.Value);
                    await callback.InvokeAsync(Runtime.FromAppSettings(), e);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPut, Route("api/RegisterCallback/{triggerId}")]
        [Trigger(TriggerType.Push, typeof(JObject))]
        [Metadata("SendGrid Webhook Item Recieved", "An item recieved in a webhook push from SendGrid")]
        public HttpResponseMessage RegisterCallback(string triggerId, [FromBody]TriggerInput<string, JObject> parameters)
        {
            CallbackStore[triggerId] = parameters.GetCallback().CallbackUri;

            return Request.PushTriggerRegistered(parameters.GetCallback());
        }

        [UnregisterCallback]
        [SwaggerResponse(HttpStatusCode.NotFound, "The trigger id had no callback registered")]
        [HttpDelete, Route("api/RegisterCallback/{triggerId}")]
        public HttpResponseMessage UnregisterCallback(string triggerId)
        {
            if (!CallbackStore.ContainsKey(triggerId))
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "The trigger id had no callback registered");

            CallbackStore.Remove(triggerId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
