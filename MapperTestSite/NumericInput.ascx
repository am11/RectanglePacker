<%@ Control Language="C#" AutoEventWireup="true" CodeFile="NumericInput.ascx.cs" Inherits="NumericInput" %>

<p>
    <asp:Label ID="lbLabel" runat="server" ></asp:Label>:
    <asp:TextBox ID="txtInput" runat="server"></asp:TextBox>
    <asp:RegularExpressionValidator ID="revInteger" runat="server" 
            ControlToValidate="txtInput" Display="Dynamic"
            ValidationExpression="\d+"
            ErrorMessage="must be numeric"><br />must be numeric</asp:RegularExpressionValidator>
    <asp:RegularExpressionValidator ID="revFloat" runat="server" 
            ControlToValidate="txtInput" Display="Dynamic"
            ValidationExpression="\d+(\.\d+)?"
            ErrorMessage="must be numeric"><br />must be numeric</asp:RegularExpressionValidator>
    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
        ControlToValidate="txtInput" Display="Dynamic"
        ErrorMessage="required"><br />required</asp:RequiredFieldValidator>
</p>

