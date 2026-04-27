<%@ Page Language="C#" CodeBehind="Launch.aspx.cs" Inherits="LtiLaunchDemo.Launch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script
      src="https://code.jquery.com/jquery-3.6.1.min.js"
      integrity="sha256-o88AwQnZB+VDvE9tvIXrMQaPlFFSUTR+nldQm1LuPXQ="
      crossorigin="anonymous"></script>

    <style type="text/css">

        * {
            margin: 0;
            padding: 0;
        }

        body {
            font-family: Calibri, sans-serif;
            color: #444;
        }

        #content {
            padding: 30px;
        }

        p { margin-bottom: 5px; }

        table.parameters {
            border-collapse: collapse;
        }

        table.parameters tr th {
            text-align: left;
        }

        table.parameters tr th, table.parameters tr td {
            padding: 5px;
        }

        table.parameters tr:nth-child(even) {
            background: #eee;
        }

        input[type="text"] {
            width: 700px;
            height: 26px;
            padding-left: 5px;
        }

        #keys {
            margin-top: 20px;
            padding-top: 20px;
            padding-bottom: 20px;
            border-top: solid 1px #aaa;
        }

        .label-debug {
            font-size: 0.5em;
            color: #d43f3a;
        }

        .button {
            display: block;
            background: #444;
            padding: 10px;
            text-align: center;
            border-radius: 5px;
            color: white;
            font-weight: bold;
            text-decoration: none;
        }

    </style>

</head>
<body>
    <form id="form1" runat="server">
        <div id="content">

            <asp:MultiView runat="server" ID="LtiProcess" ActiveViewIndex="0">
                
                <asp:View runat="server" ID="LtiStep1">

                    <h1>Simulate LTI Launch Request</h1>
            
                    <p>This form simulates a basic LTI Consumer.</p>

                    <p>Click the Validate button to create an LTI launch request for Single-Sign-On access to Shift iQ.</p>
            
                    <table class="parameters">
                        <tr>
                            <th>Parameter</th>
                            <th>Value</th>
                        </tr>
                        <tr>
                            <td><label title="user_id">Learner Code:</label></td>
                            <td><asp:TextBox runat="server" ID="LtiLearnerCode" /></td>
                        </tr>
                        <tr>
                            <td><label title="lis_person_contact_email_primary">Learner Email:</label></td>
                            <td><asp:TextBox runat="server" ID="LtiLearnerEmail" /></td>
                        </tr>
                        <tr>
                            <td><label title="lis_person_name_given">Learner First Name:</label></td>
                            <td><asp:TextBox runat="server" ID="LtiLearnerNameFirst" /></td>
                        </tr>
                        <tr>
                            <td><label title="lis_person_name_family">Learner Last Name:</label></td>
                            <td><asp:TextBox runat="server" ID="LtiLearnerNameLast" /></td>
                        </tr>
                        <tr>
                            <td><label title="shift_group_name">Group Name:</label></td>
                            <td><asp:TextBox runat="server" ID="LtiGroupName" /></td>
                        </tr>
                        <tr>
                            <td><label title="shift_organization_identifier">Organization Identifier:</label></td>
                            <td><asp:TextBox runat="server" ID="LtiOrganizationIdentifier" /></td>
                        </tr>
                        <tr>
                            <td><label title="oauth_consumer_key">Organization Secret:</label></td>
                            <td><asp:TextBox runat="server" ID="LtiOrganizationSecret" /></td>
                        </tr>
                        <tr>
                            <td><label>Launch URL:</label></td>
                            <td><asp:TextBox runat="server" ID="LtiLaunchUrl" /></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Button runat="server" ID="LtiValidate" Text="Validate" CssClass="button" Width="100%" />
                            </td>
                        </tr>
                    </table>

                </asp:View>

                <asp:View runat="server" ID="LtiStep2">

                    <h1>LTI Tool Consumer</h1>
            
                    <p>This form simulates a basic LTI Tool Consumer.</p>

                    <p>Click the Launch link to send an LTI launch request message for Single-Sign-On access to Shift iQ.</p>

                    <div id="submitForm">
                        <table class="parameters">
                            <tr>
                                <th>Parameter</th>
                                <th>Value</th>
                            </tr>
                            <asp:Repeater runat="server" ID="ParameterRepeater1">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("Key") %></td>
                                        <td><input type="text" name='<%# Eval("Key") %>' value="<%# Eval("Value") %>" /></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr>
                                <td colspan="2" align="right">
                                    <script type="text/javascript">
                                        function submitform() {
                                            var formId = "submitForm";
                                            var formHtml = "<form id='" + formId + "' action='<%= Ticket.Url %>' method='post'>" + $("#" + formId)[0].innerHTML + "</form>";

                                            $("#" + formId).replaceWith(formHtml);
                                            $("#" + formId).submit();
                                        }
                                    </script>
                                    <a class="button" href="javascript: submitform()">Launch</a>
                                </td>
                            </tr>
                        </table>
                    </div>

                    <div id="keys">
                        <asp:Repeater runat="server" ID="ParameterRepeater2">
                            <ItemTemplate>
                                <span><%# Eval("Key") %></span>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                </asp:View>

            </asp:MultiView>

        </div>
    </form>
</body>
</html>
