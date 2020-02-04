<?xml version="1.0" encoding="UTF-16"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:var="http://schemas.microsoft.com/BizTalk/2003/var" exclude-result-prefixes="msxsl var s0 userCSharp" version="1.0" xmlns:ns0="http://_x002E_Schemas.DestinationSchema.BC_InboundOrder" xmlns:s0="http://BC.Integration.Schema.ECommerce.SalesChannelOrder.New" xmlns:userCSharp="http://schemas.microsoft.com/BizTalk/2003/userCSharp">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
  <xsl:template match="/">
    <xsl:apply-templates select="/s0:Order" />
  </xsl:template>
  <xsl:template match="/s0:Order">
    <xsl:variable name="var:v1" select="userCSharp:StringConcat(&quot;HSC&quot;)" />
    <xsl:variable name="var:v2" select="userCSharp:StringConcat(&quot;INTDEV&quot;)" />
    <xsl:variable name="var:v3" select="userCSharp:StringConcat(&quot;HERSCHEL&quot;)" />
    <xsl:variable name="var:v4" select="userCSharp:StringConcat(&quot;ZZ&quot;)" />
    <xsl:variable name="var:v6" select="userCSharp:StringConcat(&quot;US&quot;)" />
    <xsl:variable name="var:v7" select="userCSharp:StringConcat(&quot;EP&quot;)" />
    <xsl:variable name="var:v9" select="userCSharp:StringConcat(&quot;Y&quot;)" />
    <ns0:Root>
      <division>
        <xsl:value-of select="$var:v1" />
      </division>
      <start_date>
        <xsl:value-of select="Header/OrderDate/text()" />
      </start_date>
      <end_date>
        <xsl:value-of select="Header/OrderDate/text()" />
      </end_date>
      <ent_user>
        <xsl:value-of select="$var:v2" />
      </ent_user>
      <our_id>
        <xsl:value-of select="$var:v3" />
      </our_id>
      <our_qual>
        <xsl:value-of select="$var:v4" />
      </our_qual>
      <xsl:if test="Header/OrderNumber">
        <po_num>
          <xsl:value-of select="Header/OrderNumber/text()" />
        </po_num>
      </xsl:if>
      <xsl:variable name="var:v5" select="userCSharp:MapCurrencyId(string(Header/CurrencyId/text()))" />
      <curr_code>
        <xsl:value-of select="$var:v5" />
      </curr_code>
      <tax_amt>
        <xsl:value-of select="Header/TaxAmount/text()" />
      </tax_amt>
      <disc_amt>
        <xsl:value-of select="Header/Discount/text()" />
      </disc_amt>
      <user_id>
        <xsl:value-of select="$var:v2" />
      </user_id>
      <vnd_id>
        <xsl:value-of select="$var:v6" />
      </vnd_id>
      <vnd_qual>
        <xsl:value-of select="$var:v7" />
      </vnd_qual>
      <location>
        <xsl:value-of select="Header/SiteId/text()" />
      </location>
      <frgt_amt>
        <xsl:value-of select="Header/Freight/text()" />
      </frgt_amt>
      <xsl:variable name="var:v8" select="userCSharp:MyConcat(string(Header/PriceCode/text()))" />
      <price_code>
        <xsl:value-of select="$var:v8" />
      </price_code>
      <auto_proc>
        <xsl:value-of select="$var:v9" />
      </auto_proc>
      <udford4c>
        <xsl:value-of select="Header/DiscountCode/text()" />
      </udford4c>
      <xsl:for-each select="Addresses/Address">
        <orderAddress>
          <addr_type>
            <xsl:value-of select="AddressType/text()" />
          </addr_type>
          <address1>
            <xsl:value-of select="Add1/text()" />
          </address1>
          <city>
            <xsl:value-of select="AddCity/text()" />
          </city>
          <country>
            <xsl:value-of select="Country/text()" />
          </country>
          <cust_name>
            <xsl:value-of select="CustomerName/text()" />
          </cust_name>
          <email>
            <xsl:value-of select="Email/text()" />
          </email>
          <phone>
            <xsl:value-of select="Phone/text()" />
          </phone>
          <state>
            <xsl:value-of select="State/text()" />
          </state>
          <zipcode>
            <xsl:value-of select="AddPostalCode/text()" />
          </zipcode>
        </orderAddress>
      </xsl:for-each>
      <xsl:for-each select="LineItems/LineItem">
        <xsl:variable name="var:v10" select="userCSharp:StringConcat(&quot;INTDEV&quot;)" />
        <orderDetail>
          <xsl:if test="UnitQuantity">
            <total_qty>
              <xsl:value-of select="UnitQuantity/text()" />
            </total_qty>
          </xsl:if>
          <xsl:if test="UnitPrice">
            <org_price>
              <xsl:value-of select="UnitPrice/text()" />
            </org_price>
          </xsl:if>
          <discount>
            <xsl:value-of select="DiscountCode/text()" />
          </discount>
          <user_id>
            <xsl:value-of select="$var:v10" />
          </user_id>
          <base_uom>
            <xsl:value-of select="UnitOfMeasure/text()" />
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

public string MapCurrencyId(string currencyId)
  {
	string code = currencyId;
         if(currencyId == "EURO")
        {
           code = "EUR";
         } 
        else if(currencyId == "KWR")
        {
           code = "KRW";
         }

       return code;
 }


public string MyConcat(string param1)
{
      string priceCode = "";

        if(param1 == "OL" ||  param1 == "RTL")
        {
             priceCode ="A";
        } 
        else if(param1=="WHSL")
        {
             priceCode ="B";
         }
         else if(param1=="DIST")
         {
             priceCode ="C";
         }

       return priceCode; 
}


public string StringConcat(string param0)
{
   return param0;
}



]]></msxsl:script>
</xsl:stylesheet>