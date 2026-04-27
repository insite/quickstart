using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;

namespace LtiLaunchDemo
{
    public partial class Launch : System.Web.UI.Page
    {
        protected LtiTicket Ticket { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LtiValidate.Click += ValidateClicked;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadDefaults();
        }

        private void LoadDefaults()
        {
            foreach (var control in LtiStep1.Controls)
                if (control is TextBox)
                    LoadDefault((TextBox)control);
        }

        private void LoadDefault(TextBox box)
        {
            box.Text = ConfigurationManager.AppSettings["Defaults." + box.ID];
        }

        private void ValidateClicked(object sender, EventArgs e)
        {
            CreateLtiTicket();

            var sortedParameters = new SortedDictionary<string, string>();

            foreach (var key in Ticket.Parameters.AllKeys)
                sortedParameters.Add(key, Ticket.Parameters[key]);

            var parameterItems = sortedParameters.Select(x => new { x.Key, x.Value });

            ParameterRepeater1.DataSource = parameterItems;
            ParameterRepeater1.DataBind();

            ParameterRepeater2.DataSource = parameterItems;
            ParameterRepeater2.DataBind();

            LtiProcess.ActiveViewIndex = 1;
        }

        private void CreateLtiTicket()
        {
            var identifier = LtiOrganizationIdentifier.Text;
            var group = LtiGroupName.Text;
            var secret = LtiOrganizationSecret.Text;
            var code = LtiLearnerCode.Text;
            var email = LtiLearnerEmail.Text;
            var firstName = LtiLearnerNameFirst.Text;
            var lastName = LtiLearnerNameLast.Text;
            var role = "Learner";

            var url = LtiLaunchUrl.Text;

            var postParameters = new LtiParameters("POST");

            postParameters.Add("oauth_consumer_key", secret);

            postParameters.Add("user_id", code);

            postParameters.Add("lis_person_name_given", firstName);
            postParameters.Add("lis_person_name_family", lastName);
            postParameters.Add("lis_person_contact_email_primary", email);

            postParameters.Add("roles", role);

            postParameters.Add("shift_group_name", group);
            postParameters.Add("shift_organization_identifier", identifier);

            Ticket = LtiTicketHelper.GetTicket(secret, url, postParameters);

            Ticket.Parameters.Add("oauth_signature", Ticket.Signature);
        }
    }
}