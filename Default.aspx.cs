﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

public partial class Default2 : System.Web.UI.Page
{
    //与微信公众账号后台的Token设置保持一致，区分大小写。
    public readonly string Token = "whchuntian";

    protected void Page_Load(object sender, EventArgs e)
    {
        Auth();
    }

    /// <summary>
    /// 处理微信服务器验证消息
    /// </summary>
    private void Auth()
    {
        string signature = Request["signature"];
        string timestamp = Request["timestamp"];
        string nonce = Request["nonce"];
        string echostr = Request["echostr"];

        if (Request.HttpMethod == "GET")
        {
            //get method - 仅在微信后台填写URL验证时触发
            if (Check(signature, timestamp, nonce, Token))
            {
                WriteContent(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                WriteContent("failed:" + signature + "," + GetSignature(timestamp, nonce, Token) + "。" +
                            "如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
            }
            Response.End();
        }
    }

    private void WriteContent(string str)
    {
        Response.Output.Write(str);
    }
    /// <summary>
    /// 检查签名是否正确
    /// </summary>
    /// <param name="signature"></param>
    /// <param name="timestamp"></param>
    /// <param name="nonce"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public  bool Check(string signature, string timestamp, string nonce, string token)
    {
        return signature == GetSignature(timestamp, nonce, token);
    }

    /// <summary>
    /// 返回正确的签名
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="nonce"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public  string GetSignature(string timestamp, string nonce, string token)
    {
        token = token ?? Token;
        string[] arr = new[] { token, timestamp, nonce }.OrderBy(z => z).ToArray();
        string arrString = string.Join("", arr);
        System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
        byte[] sha1Arr = sha1.ComputeHash(Encoding.UTF8.GetBytes(arrString));
        StringBuilder enText = new StringBuilder();
        foreach (var b in sha1Arr)
        {
            enText.AppendFormat("{0:x2}", b);
        }
        return enText.ToString();
    }
}