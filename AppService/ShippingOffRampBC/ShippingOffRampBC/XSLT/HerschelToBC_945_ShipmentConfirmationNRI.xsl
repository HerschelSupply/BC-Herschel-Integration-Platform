<?xml version="1.0" encoding="UTF-16"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:var="http://schemas.microsoft.com/BizTalk/2003/var" exclude-result-prefixes="msxsl var s0 userCSharp" version="1.0" xmlns:s0="http://BC.Integration.Schema.ShippingConfirmation.NRI" xmlns:ns0="http://Schemas.DestinationSchema.BC_ShipmentConfirmation" xmlns:userCSharp="http://schemas.microsoft.com/BizTalk/2003/userCSharp">
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
      <xsl:if test="s0:pickNum">
        <pick_num>
          <xsl:value-of select="s0:pickNum/text()" />
        </pick_num>
      </xsl:if>
      <shipper>
        <xsl:value-of select="$var:v1" />
      </shipper>
      <xsl:if test="s0:shippingDate">
        <ship_date>
          <xsl:value-of select="s0:shippingDate/text()" />
        </ship_date>
      </xsl:if>
      <xsl:if test="s0:totalWeight">
        <weight>
          <xsl:value-of select="s0:totalWeight/text()" />
        </weight>
      </xsl:if>
      <xsl:if test="s0:freight">
        <frgt_amt>
          <xsl:value-of select="s0:freight/text()" />
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
      <xsl:if test="s0:site">
        <location>
          <xsl:value-of select="s0:site/text()" />
        </location>
      </xsl:if>
      <user_id>
        <xsl:value-of select="$var:v5" />
      </user_id>
      <xsl:for-each select="s0:lineItems">
        <xsl:variable name="var:v6" select="userCSharp:StringConcat(&quot;INTDEV&quot;)" />
        <pickDetail>
          <xsl:if test="../s0:pickNum">
            <pick_num>
              <xsl:value-of select="../s0:pickNum/text()" />
            </pick_num>
          </xsl:if>
          <xsl:if test="s0:qtyShipped">
            <size_qty>
              <xsl:value-of select="s0:qtyShipped/text()" />
            </size_qty>
          </xsl:if>
          <xsl:if test="s0:itemNbr">
            <upc>
              <xsl:value-of select="s0:itemNbr/text()" />
            </upc>
          </xsl:if>
          <user_id>
            <xsl:value-of select="$var:v6" />
          </user_id>
        </pickDetail>
      </xsl:for-each>
      <xsl:for-each select="s0:cartons">
        <xsl:variable name="var:v7" select="userCSharp:StringConcat(&quot;INTDEV&quot;)" />
        <cartonHeader>
          <xsl:if test="s0:cartonNbr">
            <carton_num>
              <xsl:value-of select="s0:cartonNbr/text()" />
            </carton_num>
          </xsl:if>
          <xsl:if test="s0:cartonWeight">
            <carton_wgt>
              <xsl:value-of select="s0:cartonWeight/text()" />
            </carton_wgt>
          </xsl:if>
          <xsl:if test="../s0:pickNum">
            <pick_num>
              <xsl:value-of select="../s0:pickNum/text()" />
            </pick_num>
          </xsl:if>
          <xsl:if test="s0:trackingNumber">
            <track_no>
              <xsl:value-of select="s0:trackingNumber/text()" />
            </track_no>
          </xsl:if>
          <user_id>
            <xsl:value-of select="$var:v7" />
          </user_id>
          <xsl:for-each select="s0:cartonItems">
            <xsl:variable name="var:v9" select="userCSharp:StringConcat(&quot;INTDEV&quot;)" />
            <cartonDetail>
              <xsl:if test="s0:cartonNbr">
                <carton_num>
                  <xsl:value-of select="s0:cartonNbr/text()" />
                </carton_num>
              </xsl:if>
              <xsl:if test="../../s0:pickNum">
                <pick_num>
                  <xsl:value-of select="../../s0:pickNum/text()" />
                </pick_num>
              </xsl:if>
              <xsl:if test="s0:qtyShipped">
                <ship_qty>
                  <xsl:value-of select="s0:qtyShipped/text()" />
                </ship_qty>
              </xsl:if>
              <xsl:variable name="var:v8" select="userCSharp:MyConcat(string(s0:itemUom/text()))" />
              <shipqty_uom>
                <xsl:value-of select="$var:v8" />
              </shipqty_uom>
              <xsl:if test="s0:qtyShipped">
                <total_qty>
                  <xsl:value-of select="s0:qtyShipped/text()" />
                </total_qty>
              </xsl:if>
              <xsl:if test="s0:itemNumber">
                <product_id>
                  <xsl:value-of select="s0:itemNumber/text()" />
                </product_id>
              </xsl:if>
              <user_id>
                <xsl:value-of select="$var:v9" />
              </user_id>
            </cartonDetail>
          </xsl:for-each>
        </cartonHeader>
      </xsl:for-each>
    </ns0:Root>
  </xsl:template>
  <msxsl:script language="C#" implements-prefix="userCSharp"><![CDATA[
public string StringConcat(string param0)
{
   return param0;
}


public string MyConcat(string UOM_desc)
{
   string uom = UOM_desc;
  if(UOM_desc.ToUpper() == "EACH")
  {
       uom = "EA";
  }
	return uom ;
}



]]></msxsl:script>
</xsl:stylesheet>