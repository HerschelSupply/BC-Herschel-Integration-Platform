<?xml version="1.0" encoding="UTF-16"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:var="http://schemas.microsoft.com/BizTalk/2003/var" exclude-result-prefixes="msxsl var s0 userCSharp" version="1.0" xmlns:s0="http://BC.Integration.Schema.ECommerce.ShippingConfirmation.TigersOL" xmlns:ns0="http://Schemas.DestinationSchema.BC_ShipmentConfirmation" xmlns:userCSharp="http://schemas.microsoft.com/BizTalk/2003/userCSharp">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
  <xsl:template match="/">
    <xsl:apply-templates select="/s0:CanonicalShippingConfirmation" />
  </xsl:template>
  <xsl:template match="/s0:CanonicalShippingConfirmation">
    <xsl:variable name="var:v1" select="userCSharp:StringConcat(&quot;FXX&quot;)" />
    <xsl:variable name="var:v2" select="userCSharp:StringConcat(&quot;Y&quot;)" />
    <xsl:variable name="var:v3" select="userCSharp:StringConcat(&quot;US&quot;)" />
    <xsl:variable name="var:v4" select="userCSharp:StringConcat(&quot;EP&quot;)" />
    <xsl:variable name="var:v5" select="userCSharp:StringConcat(&quot;INTDEV&quot;)" />
    <ns0:Root>
      <xsl:if test="s0:PickNum">
        <pick_num>
          <xsl:value-of select="s0:PickNum/text()" />
        </pick_num>
      </xsl:if>
      <shipper>
        <xsl:value-of select="$var:v1" />
      </shipper>
      <xsl:if test="s0:ShippingDate">
        <ship_date>
          <xsl:value-of select="s0:ShippingDate/text()" />
        </ship_date>
      </xsl:if>
      <xsl:if test="s0:TotalWeight">
        <weight>
          <xsl:value-of select="s0:TotalWeight/text()" />
        </weight>
      </xsl:if>
      <xsl:if test="s0:Freight">
        <frgt_amt>
          <xsl:value-of select="s0:Freight/text()" />
        </frgt_amt>
      </xsl:if>
      <auto_proc>
        <xsl:value-of select="$var:v2" />
      </auto_proc>
      <vnd_id>
        <xsl:value-of select="$var:v3" />
      </vnd_id>
      <vnd_qual>
        <xsl:value-of select="$var:v4" />
      </vnd_qual>
      <user_id>
        <xsl:value-of select="$var:v5" />
      </user_id>
      <xsl:for-each select="s0:LineItems">
        <xsl:variable name="var:v6" select="userCSharp:StringConcat(&quot;INTDEV&quot;)" />
        <pickDetail>
          <xsl:if test="../s0:PickNum">
            <pick_num>
              <xsl:value-of select="../s0:PickNum/text()" />
            </pick_num>
          </xsl:if>
          <xsl:if test="s0:LineItem/s0:QtyShipped">
            <size_qty>
              <xsl:value-of select="s0:LineItem/s0:QtyShipped/text()" />
            </size_qty>
          </xsl:if>
          <xsl:if test="s0:LineItem/s0:ItemNbr">
            <upc>
              <xsl:value-of select="s0:LineItem/s0:ItemNbr/text()" />
            </upc>
          </xsl:if>
          <user_id>
            <xsl:value-of select="$var:v6" />
          </user_id>
        </pickDetail>
      </xsl:for-each>
    </ns0:Root>
  </xsl:template>
  <msxsl:script language="C#" implements-prefix="userCSharp"><![CDATA[
public string StringConcat(string param0)
{
   return param0;
}



]]></msxsl:script>
</xsl:stylesheet>