﻿using System.Collections;
using System.Net;
using System.Text;
using System.Web;

namespace Circular.Framework.ShortMessages
{
    public class BulkSMS : IBulkSMS
    {
        private object eight_bit_msg;
        private object unicode_msg;

        public string BulkSms(ShortMessageDetails shortMessageDetails, ShortMessageProviderDetails shortMessageProviderDetails)
        {

            string username = shortMessageProviderDetails.UserName;
            string password = shortMessageProviderDetails.Password;



            string msisdn = shortMessageDetails.Mobile_number;
            string seven_bit_msg = shortMessageDetails.Message;
            string url = "https://api-legacy2.bulksms.com/eapi/submission/send_sms/2/2.0";

            Hashtable result;

            int sleep_time = 3;


            int retry_growth_factor = 8;
            int num_retries = 5;

            string data = seven_bit_message(username, password, msisdn, seven_bit_msg);

            for (int x = 0; x < num_retries; x++)
            {
                result = send_sms(data, url);
                if ((int)result["success"] == 1)
                {
                    return result.ToString();
                    break;
                }
                else
                {
                    return false.ToString();
                }

                System.Threading.Thread.Sleep(sleep_time);
                sleep_time *= retry_growth_factor;
            }

            sleep_time = 3;
            data = eight_bit_message(username, password, msisdn, (string)eight_bit_msg);
            for (int x = 0; x < num_retries; x++)
            {
                result = send_sms(data, url);
                if ((int)result["success"] == 1)
                {
                    Console.WriteLine(formatted_server_response(result));
                    break;
                }
                else
                {
                    Console.WriteLine(formatted_server_response(result));
                }

                System.Threading.Thread.Sleep(sleep_time);
                sleep_time *= retry_growth_factor;
            }

            sleep_time = 3;
            data = unicode_message(username, password, msisdn, (string)unicode_msg);
            for (int x = 0; x < num_retries; x++)
            {
                result = send_sms(data, url);
                if ((int)result["success"] == 1)
                {
                    Console.WriteLine(formatted_server_response(result));
                    break;
                }
                else
                {
                    Console.WriteLine(formatted_server_response(result));
                }

                System.Threading.Thread.Sleep(sleep_time);
                sleep_time *= retry_growth_factor;
            }

            return null;
        }

        public static string formatted_server_response(Hashtable result)
        {
            string ret_string = "";
            if ((int)result["success"] == 1)
            {
                ret_string += "Success: batch ID " + (string)result["api_batch_id"] + "API message: " + (string)result["api_message"] + "\nFull details " + (string)result["details"];
            }
            else
            {
                ret_string += "Fatal error: HTTP status " + (string)result["http_status_code"] + " API status " + (string)result["api_status_code"] + " API message " + (string)result["api_message"] + "\nFull details " + (string)result["details"];
            }

            return ret_string;
        }

        public static Hashtable send_sms(string data, string url)
        {
            string sms_result = Post(url, data);

            Hashtable result_hash = new Hashtable();

            string tmp = "";
            tmp += "Response from server: " + sms_result + "\n";
            string[] parts = sms_result.Split('|');

            string statusCode = parts[0];
            string statusString = parts[1];

            result_hash.Add("api_status_code", statusCode);
            result_hash.Add("api_message", statusString);

            if (parts.Length != 3)
            {
                tmp += "Error: could not parse valid return data from server.\n";
            }
            else
            {
                if (statusCode.Equals("0"))
                {
                    result_hash.Add("success", 1);
                    result_hash.Add("api_batch_id", parts[2]);
                    tmp += "Message sent - batch ID " + parts[2] + "\n";
                }
                else if (statusCode.Equals("1"))
                {
                    // Success: scheduled for later sending.
                    result_hash.Add("success", 1);
                    result_hash.Add("api_batch_id", parts[2]);
                }
                else
                {
                    result_hash.Add("success", 0);
                    tmp += "Error sending: status code " + parts[0] + " description: " + parts[1] + "\n";
                }
            }
            result_hash.Add("details", tmp);
            return result_hash;
        }

        public static string Post(string url, string data)
        {

            string result = null;
            try
            {
                byte[] buffer = Encoding.Default.GetBytes(data);

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(url);
                WebReq.Method = "POST";
                WebReq.ContentType = "application/x-www-form-urlencoded";
                WebReq.ContentLength = buffer.Length;
                Stream PostData = WebReq.GetRequestStream();

                PostData.Write(buffer, 0, buffer.Length);
                PostData.Close();
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                Console.WriteLine(WebResp.StatusCode);

                Stream Response = WebResp.GetResponseStream();
                StreamReader _Response = new StreamReader(Response);
                result = _Response.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return result.Trim() + "\n";
        }

        public static string character_resolve(string body)
        {
            Hashtable chrs = new Hashtable();
            chrs.Add('Ω', "Û");
            chrs.Add('Θ', "Ô");
            chrs.Add('Δ', "Ð");
            chrs.Add('Φ', "Þ");
            chrs.Add('Γ', "¬");
            chrs.Add('Λ', "Â");
            chrs.Add('Π', "º");
            chrs.Add('Ψ', "Ý");
            chrs.Add('Σ', "Ê");
            chrs.Add('Ξ', "±");

            string ret_str = "";
            foreach (char c in body)
            {
                if (chrs.ContainsKey(c))
                {
                    ret_str += chrs[c];
                }
                else
                {
                    ret_str += c;
                }
            }
            return ret_str;
        }

        public static string seven_bit_message(string username, string password, string msisdn, string message)
        {

            /********************************************************************
            * Construct data                                                    *
            *********************************************************************/
            /*
            * Note the suggested encoding for the some parameters, notably
            * the username, password and especially the message.  ISO-8859-1
            * is essentially the character set that we use for message bodies,
            * with a few exceptions for e.g. Greek characters. For a full list,
            * see: https://www.bulksms.com/developer/eapi/submission/character-encoding/
            */

            string data = "";
            data += "username=" + HttpUtility.UrlEncode(username, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&password=" + HttpUtility.UrlEncode(password, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&message=" + HttpUtility.UrlEncode(character_resolve(message), System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&msisdn=" + msisdn;
            data += "&want_report=1";

            return data;
        }

        public static string unicode_message(string username, string password, string msisdn, string message)
        {

            /********************************************************************
            * Construct data                                                    *
            *********************************************************************/
            /*
            * Note the suggested encoding for the some parameters, notably
            * the username, password and especially the message.  ISO-8859-1
            * is essentially the character set that we use for message bodies,
            * with a few exceptions for e.g. Greek characters. For a full list,
            * see: https://www.bulksms.com/developer/eapi/submission/character-encoding/
            */


            string data = "";
            data += "username=" + HttpUtility.UrlEncode(username, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&password=" + HttpUtility.UrlEncode(password, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&message=" + stringToHex(message);
            data += "&msisdn=" + msisdn;
            data += "&dca=16bit";
            data += "&want_report=1";

            return data;
        }

        public static string eight_bit_message(string username, string password, string msisdn, string message)
        {

            /********************************************************************
            * Construct data                                                    *
            *********************************************************************/
            /*
            * Note the suggested encoding for the some parameters, notably
            * the username, password and especially the message.  ISO-8859-1
            * is essentially the character set that we use for message bodies,
            * with a few exceptions for e.g. Greek characters. For a full list,
            * see: https://www.bulksms.com/developer/eapi/submission/character-encoding/
            */

            /*
            * In the following $udh string, 0B84 is a destination port and 23F0 is the origin port.
            */
            string udh = "0605040B8423F0";

            /*
            * $wsp_header is broken down into the following:
            *
            * DC - Transaction ID (used to associate PDUs)
            * 06 - PDU type (push PDU)
            * 01 - HeadersLen (total of content-type and headers, i.e. zero headers)
            * AE - Content Type: application/vnd.wap.sic
            */
            string wsp_header = "DC0601AE";
            string wap_push_message = udh + wsp_header + message;

            string data = "";
            data += "username=" + HttpUtility.UrlEncode(username, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&password=" + HttpUtility.UrlEncode(password, System.Text.Encoding.GetEncoding("ISO-8859-1"));
            data += "&message=" + wap_push_message;
            data += "&msisdn=" + msisdn;
            data += "&dca=8bit";
            data += "&want_report=1";

            return data;
        }

        public static string get_headers(string msg_type)
        {
            string headers = "";
            if (msg_type == "wap_push")
            {
                string udh = "0605040B8423F0";
                string wsp = "DC0601AE";

                headers += udh + wsp;
            }
            else if (msg_type == "vCard" || msg_type == "vCalendar")
            {
                headers += "06050423F40000";
            }
            return headers;
        }

        public static string xml_to_string(string msg_body)
        {
            //TODO
            /*
            * Code to convert 'msg_body' will go in here.
            */

            return "conversion";
        }

        public static string stringToHex(string s)
        {
            string hex = "";
            foreach (char c in s)
            {
                int tmp = c;
                hex += String.Format("{0:x4}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }
    }
}
