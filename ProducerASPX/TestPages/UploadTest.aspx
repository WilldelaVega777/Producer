<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadTest.aspx.cs" Inherits="WayvSL2ASPX.TestPages.UploadTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <p style="font-family: Tahoma; font-size: medium">
        Upload File:
        <asp:FileUpload ID="OFileUpload" runat="server" />
&nbsp;&nbsp;
        <asp:Button ID="OSubmitButton" runat="server" onclick="OSubmitButton_Click" 
            Text="Submit!" />
    </p>
    <p>
        <asp:Label ID="lblError" runat="server" Font-Names="Tahoma" Font-Size="Small" 
            ForeColor="#993300" Text="[If there is any error, it will appear here..]"></asp:Label>
    </p>
    </form>
</body>
</html>
