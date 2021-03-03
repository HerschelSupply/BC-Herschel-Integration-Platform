<?xml version="1.0" encoding="UTF-16"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:var="http://schemas.microsoft.com/BizTalk/2003/var" exclude-result-prefixes="msxsl var s0 userCSharp" version="1.0" xmlns:ns0="http://Schemas.DestinationSchema.BC_Return" xmlns:s0="http://BC.Integration.Schema.ECommerce.SalesChannelOrder" xmlns:userCSharp="http://schemas.microsoft.com/BizTalk/2003/userCSharp">
  <xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />
  <xsl:template match="/">
    <xsl:apply-templates select="/s0:Return" />
  </xsl:template>
  <xsl:template match="/s0:Return">
    <xsl:variable name="var:v1" select="userCSharp:StringConcat(&quot;HSC&quot;)" />
    <xsl:variable name="var:v2" select="userCSharp:StringConcat(&quot;RETURN&quot;)" />
    <xsl:variable name="var:v4" select="userCSharp:StringConcat(&quot;C&quot;)" />
    <xsl:variable name="var:v5" select="userCSharp:StringConcat(&quot;P&quot;)" />
    <xsl:variable name="var:v6" select="userCSharp:StringConcat(&quot;HERSCHEL&quot;)" />
    <xsl:variable name="var:v7" select="userCSharp:StringConcat(&quot;EP&quot;)" />
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
      <division>
        <xsl:value-of select="$var:v1" />
      </division>
      <xsl:if test="Header/CancelDate">
        <ent_date>
          <xsl:value-of select="Header/CancelDate/text()" />
        </ent_date>
      </xsl:if>
      <insu_amt>
        <xsl:text>0.0</xsl:text>
      </insu_amt>
      <xsl:if test="Header/SiteId">
        <location>
          <xsl:value-of select="Header/SiteId/text()" />
        </location>
      </xsl:if>
      <misc_amt>
        <xsl:text>0.0</xsl:text>
      </misc_amt>
      <xsl:if test="ReturnLineItems/LineItem/RmaCode">
        <ref_num>
          <xsl:value-of select="ReturnLineItems/LineItem/RmaCode/text()" />
        </ref_num>
      </xsl:if>
      <xsl:if test="Header/OrderNumber">
        <ref_num2>
          <xsl:value-of select="Header/OrderNumber/text()" />
        </ref_num2>
      </xsl:if>
      <retn_type>
        <xsl:value-of select="$var:v2" />
      </retn_type>
      <xsl:variable name="var:v3" select="userCSharp:returnReasonCode(string(ReturnLineItems/LineItem/SKU/ReturnReason/text()))" />
      <rsn_code>
        <xsl:value-of select="$var:v3" />
      </rsn_code>
      <rsn_type>
        <xsl:value-of select="$var:v4" />
      </rsn_type>
      <tax_amt>
        <xsl:value-of select="Header/TaxAmount/text()" />
      </tax_amt>
      <tax_empt>
        <xsl:value-of select="$var:v5" />
      </tax_empt>
      <vnd_id>
        <xsl:value-of select="$var:v6" />
      </vnd_id>
      <vnd_qual>
        <xsl:value-of select="$var:v7" />
      </vnd_qual>
      <xsl:for-each select="Addresses">
        <xsl:variable name="var:v8" select="userCSharp:StringConcat(&quot;RF&quot;)" />
        <returnsHeaderAddress>
          <addr_type>
            <xsl:value-of select="$var:v8" />
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
      <ReturnsDetail>
        <xsl:for-each select="ReturnLineItems">
          <xsl:for-each select="LineItem/SKU">
            <xsl:variable name="var:v9" select="userCSharp:StringConcat(&quot;&quot;)" />
            <xsl:variable name="var:v11" select="userCSharp:StringConcat(&quot;C&quot;)" />
            <xsl:variable name="var:v12" select="userCSharp:StringConcat(&quot;EA&quot;)" />
            <returnsDetail>
              <claim_price>
                <xsl:value-of select="ReturnAmount/text()" />
              </claim_price>
              <uom>
                <xsl:value-of select="$var:v9" />
              </uom>
              <price>
                <xsl:value-of select="UnitPrice/text()" />
              </price>
              <xsl:if test="ReturnItemCode">
                <product_id>
                  <xsl:value-of select="ReturnItemCode/text()" />
                </product_id>
              </xsl:if>
              <xsl:variable name="var:v10" select="userCSharp:returnReasonCode(string(ReturnReason/text()))" />
              <rsn_code>
                <xsl:value-of select="$var:v10" />
              </rsn_code>
              <rsn_type>
                <xsl:value-of select="$var:v11" />
              </rsn_type>
              <upc>
                <xsl:value-of select="$var:v9" />
              </upc>
              <xsl:if test="Quantity">
                <total_qty>
                  <xsl:value-of select="Quantity/text()" />
                </total_qty>
              </xsl:if>
              <ib_uom>
                <xsl:value-of select="$var:v12" />
              </ib_uom>
              <inv_num>
                <xsl:value-of select="../OrderInvoiceNumber/text()" />
              </inv_num>
            </returnsDetail>
          </xsl:for-each>
        </xsl:for-each>
      </ReturnsDetail>
    </ns0:Root>
  </xsl:template>
  <msxsl:script language="C#" implements-prefix="userCSharp"><![CDATA[
public string StringConcat(string param0)
{
   return param0;
}


    public  string returnReasonCode(string ReturnReason)
        {
            string reason = "";
            switch (ReturnReason)
            {
                case "OrderReturnSkuReason_ChangedMind":
                    reason = "E01";
                    break;
                case "OrderReturnSkuReason_DifferentFromThePhoto":
                    reason = "E02";
                    break;
                case "OrderReturnSkuReason_Faulty":
                    reason = "E03";
                    break;
                case "OrderReturnSkuReason_IncorrectItem":
                    reason = "E04";
                    break;
                case "OrderReturnSkuReason_LateDelivery":
                    reason = "E05";
                    break;
                case "OrderReturnSkuReason_UnwantedGift":
                    reason = "E06";
                    break;
                case "OrderReturnSkuReason_WrongSizeTooLarge":
                    reason = "E07";
                    break;
                case "OrderReturnSkuReason_WrongSizeTooSmall":
                    reason = "E08";
                    break;
            }
            return reason;
        }


]]></msxsl:script>
</xsl:stylesheet>