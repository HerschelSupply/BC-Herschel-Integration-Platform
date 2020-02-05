<?xml version="1.0" encoding="UTF-16"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:var="http://schemas.microsoft.com/BizTalk/2003/var" exclude-result-prefixes="msxsl var s0 userCSharp" version="1.0" xmlns:s0="http://Corp.Integration.Schema.ECommerce.SalesChannelOrder" xmlns:ns0="http://Schemas.DestinationSchema.BC_Return" xmlns:userCSharp="http://schemas.microsoft.com/BizTalk/2003/userCSharp">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
  <xsl:template match="/">
    <xsl:apply-templates select="/s0:Return" />
  </xsl:template>
  <xsl:template match="/s0:Return">
    <xsl:variable name="var:v1" select="userCSharp:StringConcat(&quot;E01&quot;)" />
    <xsl:variable name="var:v2" select="userCSharp:StringConcat(&quot;P&quot;)" />
    <ns0:Root>
      <xsl:if test="Header/CustomerId">
        <customer>
          <xsl:value-of select="Header/CustomerId/text()" />
        </customer>
      </xsl:if>
      <xsl:if test="Header/Discount">
        <disc_amt>
          <xsl:value-of select="Header/Discount/text()" />
        </disc_amt>
      </xsl:if>
      <xsl:if test="ReturnLineItems/LineItem/RmaCode">
        <doc_num>
          <xsl:value-of select="ReturnLineItems/LineItem/RmaCode/text()" />
        </doc_num>
      </xsl:if>
      <xsl:if test="Header/CancelDate">
        <ent_date>
          <xsl:value-of select="Header/CancelDate/text()" />
        </ent_date>
      </xsl:if>
      <insu_amt>
        <xsl:text>0.0</xsl:text>
      </insu_amt>
      <misc_amt>
        <xsl:text>0.0</xsl:text>
      </misc_amt>
      <xsl:if test="Header/OrderNumber">
        <ref_num>
          <xsl:value-of select="Header/OrderNumber/text()" />
        </ref_num>
      </xsl:if>
      <rsn_code>
        <xsl:value-of select="$var:v1" />
      </rsn_code>
      <tax_amt>
        <xsl:value-of select="Header/TaxAmount/text()" />
      </tax_amt>
      <tax_empt>
        <xsl:value-of select="$var:v2" />
      </tax_empt>
      <xsl:for-each select="Addresses">
        <xsl:variable name="var:v3" select="userCSharp:StringConcat(&quot;RF&quot;)" />
        <returnsHeaderAddress>
          <addr_type>
            <xsl:value-of select="$var:v3" />
          </addr_type>
          <xsl:if test="Address/Add1">
            <address1>
              <xsl:value-of select="Address/Add1/text()" />
            </address1>
          </xsl:if>
          <xsl:if test="Address/Add2">
            <address2>
              <xsl:value-of select="Address/Add2/text()" />
            </address2>
          </xsl:if>
          <address3>
            <xsl:value-of select="Address/Add3/text()" />
          </address3>
          <city>
            <xsl:value-of select="Address/AddCity/text()" />
          </city>
          <country>
            <xsl:value-of select="Address/Country/text()" />
          </country>
          <state>
            <xsl:value-of select="Address/State/text()" />
          </state>
          <zipcode>
            <xsl:value-of select="Address/AddPostalCode/text()" />
          </zipcode>
        </returnsHeaderAddress>
      </xsl:for-each>
      <Returns>
        <xsl:for-each select="ReturnLineItems">
          <xsl:for-each select="LineItem/SKU">
            <xsl:variable name="var:v4" select="userCSharp:StringConcat(&quot;&quot;)" />
            <xsl:variable name="var:v5" select="userCSharp:StringConcat(&quot;E01&quot;)" />
            <xsl:variable name="var:v6" select="userCSharp:StringConcat(&quot;EA&quot;)" />
            <returnsDetail>
              <claim_price>
                <xsl:value-of select="ReturnAmount/text()" />
              </claim_price>
              <uom>
                <xsl:value-of select="$var:v4" />
              </uom>
              <xsl:if test="../RmaCode">
                <doc_num>
                  <xsl:value-of select="../RmaCode/text()" />
                </doc_num>
              </xsl:if>
              <price>
                <xsl:value-of select="UnitPrice/text()" />
              </price>
              <xsl:if test="ReturnItemCode">
                <product_id>
                  <xsl:value-of select="ReturnItemCode/text()" />
                </product_id>
              </xsl:if>
              <rsn_code>
                <xsl:value-of select="$var:v5" />
              </rsn_code>
              <sku>
                <xsl:value-of select="$var:v4" />
              </sku>
              <upc>
                <xsl:value-of select="$var:v4" />
              </upc>
              <xsl:if test="Quantity">
                <total_qty>
                  <xsl:value-of select="Quantity/text()" />
                </total_qty>
              </xsl:if>
              <ib_uom>
                <xsl:value-of select="$var:v6" />
              </ib_uom>
            </returnsDetail>
          </xsl:for-each>
        </xsl:for-each>
      </Returns>
    </ns0:Root>
  </xsl:template>
  <msxsl:script language="C#" implements-prefix="userCSharp"><![CDATA[
public string StringConcat(string param0)
{
   return param0;
}



]]></msxsl:script>
</xsl:stylesheet>