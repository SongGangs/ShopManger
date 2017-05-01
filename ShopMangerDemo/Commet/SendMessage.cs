using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace ShopMangerDemo
{
    class SendMessage
    {
        private string url = "http://gw.api.taobao.com/router/rest?"; //阿里服务网址
        private string AppKey = "23281692";
        private string Security = "e767dfa2ce0a46ba554268dc720c7be6";
        private static int SecurityCode; //验证码
        

        //发送验证码
        private string SendSecurityCode(string SecurityCode, string phoneNumber)
        {
            string result = "";
            if (MetarnetRegex.IsMobilePhone(phoneNumber))
            {
                ITopClient client = new DefaultTopClient(url, AppKey, Security);

                AlibabaAliqinFcSmsNumSendRequest req = new AlibabaAliqinFcSmsNumSendRequest();

                req.Extend = "123456";

                req.SmsType = "normal";

                req.SmsFreeSignName = "注册验证";
                //    req.SmsParam = String.Format("{\"code\":\"1234\",\"product\":\"打堆云打印\"}", SecurityCode);// "{\"code\":\"1234\",\"product\":\"打堆云打印\"}";
                req.SmsParam = "{\"code\":\"" + SecurityCode + "\",\"product\":\"宋刚之云商行\"}";
                req.RecNum = phoneNumber;
                req.SmsTemplateCode = "SMS_3125049";
                AlibabaAliqinFcSmsNumSendResponse rsp = client.Execute(req);
                result = rsp.Body;
            }
            return result;
        }

        public  string SendMessages(string phoneNumber)
        {
            Random rd = new Random();
            SecurityCode = rd.Next(1234, 9879); //生成验证码              
            SendSecurityCode(SecurityCode.ToString(),phoneNumber); //发送验证码，后面还要根据返回的值判断是否发送成功
            
            return SecurityCode.ToString();
        }
    }
}
