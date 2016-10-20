using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace AsiaLabv1.Services
{
    public class SMSService
    {
        public void SendSms(string PhoneNumber,int ReciptNo)
        {
            // anzulaqueel.com POST URL
            string url = "http://anzulaqueel.com/json.php?username=AsiaLab&password=asialab100";
            // XML-formatted data

            string senderSender = "Asia Lab";
            string mobileMobile = PhoneNumber;
            string messageMessage = "Thank you for visiting Asia Lab, your Receipt no. is " + ReciptNo + ". " + "\n" + DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToShortTimeString();


            string fields = "&to=" + mobileMobile +
            "&from=" + senderSender + "&message=" + messageMessage;

            url = url + fields;

            // web request start

            Uri uri = new Uri(url);
            string data = "field-keywords=ASP.NET4.0";

            if (uri.Scheme == Uri.UriSchemeHttp)
            {
                // create a request on behalf of uri
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                // setting parameter for the request
                request.Method = WebRequestMethods.Http.Post;
                request.ContentLength = data.Length;
                request.ContentType = "application/x-www-form-urlencoded";
                // a stream writer for the request
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                // write down the date
                writer.Write(data);
                //close the stream writer
                writer.Close();
                // GET / POSTting response from the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // GET / POST the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();
                // Pipes the stream to a higher level stream reader with the required encoding format.
                StreamReader readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8);


                // to write a http response from the characters

                //Response.Write(readStream.ReadToEnd());
                // close the response

                response.Close();
                // close the reader

                readStream.Close();
            }
        }
    }
}