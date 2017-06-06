<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SEO_Analyser.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Seo Analyzer</title>
        <style>
            .container {
                width: 75%;
                margin-left: auto;
                margin-right: auto;
            }
    </style>
</head>
<body>
     <div class="container">
    <form id="form1" runat="server">
            SEO Analyser<br/><br/>
      <div>
        <asp:RadioButtonList id="inputTypes" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="inputTypes_SelectedIndexChanged">
            <asp:ListItem Selected="True" Value="text">Plain Text</asp:ListItem>
            <asp:ListItem Value="url">Url</asp:ListItem>
         </asp:RadioButtonList>
      </div>

    <div>
        <asp:TextBox ID="txtInput" runat="server" Rows="5" Width="500px" TextMode="MultiLine"></asp:TextBox>
    </div>
    <br />
    <div>
        <asp:CheckBox ID="chkKeys" runat="server" checked="true" Text="Include number of occurrences of each word" />
        <br />
        <asp:CheckBox ID="chkMtKey" runat="server" Enabled="false" checked="false" Text="Include number of occurrences of each word in Meta Tags" />            
        <br />
        <asp:CheckBox ID="chkLinks" runat="server" checked="true" Text="Include number of external links" /> 
        <br />
        <asp:CheckBox ID="chkStopwords" runat="server" checked="true" Text="Filter out stop-words" /> 
    </div>
        <br />
        <asp:Button ID="btnSend" runat="server" Text="Submit" OnClick="btnSend_Click" />
        <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click" />
        <br /><br />
<asp:Label ID="lblErrorMessage" runat="server" Visible="false" ForeColor="Red"></asp:Label><br /><br />
        <asp:GridView ID="gvMetaTags" Width="50%" runat="server" CellPadding="4" AutoGenerateColumns="false" ForeColor="#333333" GridLines="None" AllowSorting="true" onsorting="gvData_Sorting">
             <Columns>
                <asp:TemplateField HeaderText="Meta Tag Word" SortExpression="SortbyWord">
                    <ItemTemplate>
                        <asp:Label ID="lblWord" runat="server" Text='<%# Bind("Key") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Count" SortExpression="SortbyCount">
                    <ItemTemplate>
                        <asp:Label ID="lblCount" runat="server" Text='<%# Bind("Value") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <AlternatingRowStyle BackColor="White" />
            <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
            <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
            <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
            <SortedAscendingCellStyle BackColor="#FDF5AC" />
            <SortedAscendingHeaderStyle BackColor="#4D0000" />
            <SortedDescendingCellStyle BackColor="#FCF6C0" />
            <SortedDescendingHeaderStyle BackColor="#820000" />
            </asp:GridView>
        <br /><br />
        <asp:Label ID="lblTotLinks" runat="server" Visible="false"></asp:Label><br />
        <asp:Label ID="lblTotWords" runat="server" Visible="false"></asp:Label><br />        
<asp:gridview runat="server" ID="gvWords" CellPadding="4" AutoGenerateColumns="false" ForeColor="#333333" Width="70%" AllowSorting="true" GridLines="None" onsorting="gvData_Sorting">
    <Columns>
        <asp:TemplateField HeaderText="Word" SortExpression="SortbyWord">
            <EditItemTemplate>
                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Key") %>'></asp:TextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:Label ID="lblWord" runat="server" Text='<%# Bind("Key") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Count" SortExpression="SortbyCount">
            <ItemTemplate>
                <asp:Label ID="lblCount" runat="server" Text='<%# Bind("Value") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
    <EditRowStyle BackColor="#999999" />
    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
    <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
    <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
    <SortedAscendingCellStyle BackColor="#E9E7E2" />
    <SortedAscendingHeaderStyle BackColor="#506C8C" />
    <SortedDescendingCellStyle BackColor="#FFFDF8" />
    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
</asp:gridview>

    </form>
</div>
</body>
</html>
