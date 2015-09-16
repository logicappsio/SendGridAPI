# SendGrid API App
[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

## Deploying ##
Click the "Deploy to Azure" button above.  You can create new resources or reference existing ones (resource group, gateway, service plan, etc.)  **Site Name and Gateway must be unique URL hostnames.**  The deployment script will deploy the following:
 * Resource Group (optional)
 * Service Plan (if you don't reference exisiting one)
 * Gateway (if you don't reference existing one)
 * API App (SendGridAPI)
 * API App Host (this is the site behind the api app that this github code deploys to)
 
 ### Deploying From Visual Studio ###
 If you clone this code and publish as an API App, you need to input your SendGrid API Key into the web.config file under the key "SengridApiKey".

## API Documentation ##
The app has one action (Send Email) and one trigger (Recieve SendGrid Webhook)

### Send Email Action ###
The send email action has the following inputs

| Input | Description |
| ----- | ----- |
| Message | The plain-text message for the email |
| Recipients | Comma-separated list of email recipients |
| From | Email from address |
| Subject | Email subject |
| HTML Message | The HTML-encoded email |
| Click Tracking | If SendGrid click-tracking should be enabled for email |

### SendGrid Webhook Trigger ###

You can see documentation on the SendGrid Webhook [here](https://sendgrid.com/docs/API_Reference/Webhooks/event.html).  It will send a JSON Array of events to any HTTP Address.

If you add the SendGrid API as a trigger, it will fire off with each JSON Object the SendGrid Webhook sends.  This means that if SendGrid sends an array of 10 events, the trigger will fire once per event (10xs in this case).

In order for the webhook to function, you need to make sure the API App permission is set to "public (anonymous)", and set your SendGrid Event URL in your SendGrid account to https://{APIAppHostUrl}/webhook 
To find the API App Host URL, in Azure go to Browse -> API Apps -> SendGridAPI -> Click the link under "Host" or "Api App Host" -> You should see the URL there.  **Make sure to you add to sendgrid as HTTPS and not HTTP**
