<?xml version="1.0" encoding="UTF-16"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:var="http://schemas.microsoft.com/BizTalk/2003/var" exclude-result-prefixes="msxsl var s0 userCSharp" version="1.0" xmlns:ns0="http://_x002E_Schemas.DestinationSchema.BC_InboundOrder" xmlns:s0="http://BC.Integration.Schema.ECommerce.RetailChannelInvoice" xmlns:userCSharp="http://schemas.microsoft.com/BizTalk/2003/userCSharp">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
  <xsl:template match="/">
    <xsl:apply-templates select="/s0:Invoice" />
  </xsl:template>
  <xsl:template match="/s0:Invoice">
    <xsl:variable name="var:v1" select="userCSharp:StringConcat(&quot;HSC&quot;)" />
    <xsl:variable name="var:v2" select="userCSharp:StringConcat(&quot;HERSCHEL&quot;)" />
    <xsl:variable name="var:v3" select="userCSharp:StringConcat(&quot;ZZ&quot;)" />
    <xsl:variable name="var:v4" select="userCSharp:MathInt(&quot;13&quot;)" />
    <xsl:variable name="var:v7" select="userCSharp:StringConcat(&quot;INTDEV&quot;)" />
    <xsl:variable name="var:v8" select="userCSharp:StringConcat(&quot;C51&quot;)" />
    <xsl:variable name="var:v10" select="userCSharp:StringConcat(&quot;A&quot;)" />
    <xsl:variable name="var:v11" select="userCSharp:StringConcat(&quot;Y&quot;)" />
    <xsl:variable name="var:v12" select="userCSharp:StringConcat(&quot;WAREHOUSE&quot;)" />
    <ns0:Root>
      <division>
        <xsl:value-of select="$var:v1" />
      </division>
      <start_date>
        <xsl:value-of select="Header/InvoiceDate/text()" />
      </start_date>
      <end_date>
        <xsl:value-of select="Header/InvoiceDate/text()" />
      </end_date>
      <pri_date>
        <xsl:value-of select="Header/InvoiceDate/text()" />
      </pri_date>
      <our_id>
        <xsl:value-of select="$var:v2" />
      </our_id>
      <our_qual>
        <xsl:value-of select="$var:v3" />
      </our_qual>
      <xsl:if test="Header/InvoiceNumber">
        <po_num>
          <xsl:value-of select="Header/InvoiceNumber/text()" />
        </po_num>
      </xsl:if>
      <curr_code>
        <xsl:value-of select="Header/Currency/text()" />
      </curr_code>
      <xsl:variable name="var:v5" select="userCSharp:CalcTaxAmt(string($var:v4) , string(Header/TotalAmount/text()))" />
      <xsl:variable name="var:v6" select="userCSharp:MathRound(string($var:v5) , &quot;2&quot;)" />
      <tax_amt>
        <xsl:value-of select="$var:v6" />
      </tax_amt>
      <disc_amt>
        <xsl:text>0</xsl:text>
      </disc_amt>
      <user_id>
        <xsl:value-of select="$var:v7" />
      </user_id>
      <vnd_id>
        <xsl:value-of select="$var:v8" />
      </vnd_id>
      <vnd_qual>
        <xsl:value-of select="$var:v3" />
      </vnd_qual>
      <xsl:variable name="var:v9" select="userCSharp:MyConcat(string(Header/SiteId/text()))" />
      <location>
        <xsl:value-of select="$var:v9" />
      </location>
      <xsl:if test="Header/FreightTotal">
        <frgt_amt>
          <xsl:value-of select="Header/FreightTotal/text()" />
        </frgt_amt>
      </xsl:if>
      <price_code>
        <xsl:value-of select="$var:v10" />
      </price_code>
      <auto_proc>
        <xsl:value-of select="$var:v11" />
      </auto_proc>
      <store>
        <xsl:value-of select="$var:v12" />
      </store>
      <xsl:for-each select="LineItems/LineItem">
        <xsl:variable name="var:v14" select="userCSharp:StringConcat(&quot;INTDEV&quot;)" />
        <xsl:variable name="var:v15" select="userCSharp:StringConcat(&quot;EA&quot;)" />
        <orderDetail>
          <xsl:if test="Quantity">
            <total_qty>
              <xsl:value-of select="Quantity/text()" />
            </total_qty>
          </xsl:if>
          <xsl:if test="UnitPrice">
            <org_price>
              <xsl:value-of select="UnitPrice/text()" />
            </org_price>
          </xsl:if>
          <xsl:variable name="var:v13" select="userCSharp:MyConcat(string(Discount/text()) , string(UnitPrice/text()))" />
          <discount>
            <xsl:value-of select="$var:v13" />
          </discount>
          <user_id>
            <xsl:value-of select="$var:v14" />
          </user_id>
          <base_uom>
            <xsl:value-of select="$var:v15" />
          </base_uom>
          <xsl:if test="ItemNumber">
            <product_id>
              <xsl:value-of select="ItemNumber/text()" />
            </product_id>
          </xsl:if>
        </orderDetail>
      </xsl:for-each>
    </ns0:Root>
  </xsl:template>
  <msxsl:script language="C#" implements-prefix="userCSharp"><![CDATA[
public string StringConcat(string param0)
{
   return param0;
}



public string MyConcat(decimal discount, decimal unitPrice)
{
   if(discount == 0)
   {
        return "";
    }
    else
    {
       decimal discPercentage = Math.Abs(discount) * 100 / unitPrice ;
       return discPercentage.ToString();
     }
}



public string MyConcat(string siteId)
{
  return "C"+siteId;

}


public string MathRound(string val)
{
	return MathRound(val, "0");
}

public string MathRound(string val, string decimals)
{
	string retval = "";
	double v = 0;
	double db = 0;
	if (IsNumeric(val, ref v) && IsNumeric(decimals, ref db))
	{
		try
		{
			int d = (int) db;
			double ret = Math.Round(v, d);
			retval = ret.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
		catch (Exception)
		{
		}
	}
	return retval;
}



public decimal CalcTaxAmt(decimal taxRate, decimal totalValue)
{
                taxRate = 1 +( taxRate/100);
	return totalValue - (totalValue/taxRate);
}


public string MathInt(string val)
{
	string retval = "";
	double d = 0;
	if (IsNumeric(val, ref d))
	{
		try
		{
			int i = Convert.ToInt32(d, System.Globalization.CultureInfo.InvariantCulture);
			if (i > d)
			{
				i = i-1;
			}
			retval = i.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}
		catch (Exception)
		{
		}
	}
	return retval;
}


public bool IsNumeric(string val)
{
	if (val == null)
	{
		return false;
	}
	double d = 0;
	return Double.TryParse(val, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d);
}

public bool IsNumeric(string val, ref double d)
{
	if (val == null)
	{
		return false;
	}
	return Double.TryParse(val, System.Globalization.NumberStyles.AllowThousands | System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d);
}


]]></msxsl:script>
</xsl:stylesheet>