using System;
using System.Text;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.Domain.Concrete
{
    public class EmailHtml
    {
        // Customer Signup email

        public static string SignupEmail(int customerId, 
                                        string customerFirstName, 
                                        string customerEmail,
                                        string customerPassword,
                                        string storefrontName,
                                        string storefrontContact,
                                        string storefrontFax,
                                        string storefrontUrl,
                                        string storefrontEmailHeader,
                                        string storefrontEnquiry
                                       )
        {
            StringBuilder mailBody = new StringBuilder("");
            mailBody.Append("<html>");
            
            mailBody.Append("<head>");
         
            mailBody.Append("<title>");
            mailBody.Append("Thank You for signing up with");
            mailBody.Append(" www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("</title>");

            mailBody.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\">");
            mailBody.Append("<style type=\"text/css\"> Body  { font-size : 9pt; font-family:arial,sans-serif; font-weight : normal; font-style : normal; color : #3F3F3F; }");
            mailBody.Append("p { font-size : 9pt; font-family:arial,sans-serif; font-weight : normal; font-style : normal; color : #3F3F3F; }");
            mailBody.Append("td { font-size : 9pt; font-family:arial,sans-serif; font-weight : normal; font-style : normal; color : #3F3F3F; }");

            mailBody.Append("</style>");

            mailBody.Append("</head>");
            
            mailBody.Append(
                "<body leftmargin=\"0\" topmargin=\"0\" marginwidth=\"0\" marginheight=\"0\" alink=\"2F435E\" vlink=\"61B7E6\">");
            
            mailBody.Append("<div align=\"center\">");

            mailBody.Append("<table bgcolor=\"#FFFFFF\" width=\"688\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-top:4px #032525 solid;border-bottom:4px #032525 solid;border-left:4px #032525 solid;border-right:4px #032525 solid\">");

            mailBody.Append("<tr>");
            mailBody.Append(" <td colspan=\"2\"><a href=\"\"><img src=\"http://www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("/content/images/email/");
            mailBody.Append(storefrontEmailHeader);
            mailBody.Append(" \"border=\"0\" ></a>");
            mailBody.Append("</td>");
            mailBody.Append("</tr>");


            mailBody.Append("<tr height = \"20\">");
            mailBody.Append(" <td align=\"left\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Dear ");
            mailBody.Append(customerFirstName + ",") ;
            mailBody.Append("</td>");
            mailBody.Append("<td align=\"right\">");
            mailBody.Append(DateTime.Now.ToString("MMMM, dd, yyyy"));
            mailBody.Append("&nbsp;&nbsp;&nbsp;&nbsp;");
            mailBody.Append("</td>");
            mailBody.Append("</tr>");


            mailBody.Append("<tr>");
            mailBody.Append("<td valign=\"top\" colspan=\"2\">");
            mailBody.Append("<table bgcolor=\"FFFFFF\" cellpadding=\"0\" cellspacing=\"0\">");
         
            mailBody.Append("<tr>");
            
            mailBody.Append("<td><img src=\"http://www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("/content/images/email/spacer.gif\" width=\"20\" height=\"0\">");
            mailBody.Append("</td>");

            mailBody.Append("<td>");
            mailBody.Append("<br><br><p><font style=\"color: #2F435E; font-size: 18px;\"><b>Welcome to ");
            mailBody.Append(storefrontUrl);
            mailBody.Append("</b></font></p>");

            mailBody.Append("<p align=\"justify\">We would like to thank you for signing up with ");
            mailBody.Append(storefrontUrl);
            mailBody.Append("<br/> <br/>");
            mailBody.Append("In order to receive our confirmation emails, we suggest that you add <b>");
            mailBody.Append(storefrontEnquiry);
            mailBody.Append("</b> to your address book.<br><br>");
            mailBody.Append("Here is your customer number for your account:<br><br>");

            mailBody.Append("<table style=\"border-top:1px #032525 solid;border-bottom:1px #032525 solid;border-left:1px #032525 solid;border-right:1px #032525 solid\">");
            mailBody.Append("<tr>");
            mailBody.Append("<td>");
            mailBody.Append("Your customer number is: ");
            mailBody.Append("</td>");

            mailBody.Append("<td align=\"right\">");
            mailBody.Append("<b>");
            mailBody.Append(customerId);
            mailBody.Append("</b>");
            mailBody.Append("</td>");
            mailBody.Append("</tr>");


            mailBody.Append("<tr>");
            mailBody.Append("<td>");
            mailBody.Append("Your email address is: ");
            mailBody.Append("</td>");
            mailBody.Append("<td align=\"right\">");
            mailBody.Append("<b>");
            mailBody.Append(customerEmail);
            mailBody.Append("</b>");
            mailBody.Append("</td>");
            mailBody.Append("</tr>");

            mailBody.Append("<tr>");
            mailBody.Append("<td>");
            mailBody.Append("Your password is: ");
            mailBody.Append("</td>");
            mailBody.Append("<td align=\"right\">");
            mailBody.Append("<b>");
            mailBody.Append(customerPassword);
            mailBody.Append("</b>");
            mailBody.Append("</td>");
            mailBody.Append("</tr>");

            mailBody.Append("</table> <br/> <br/>");


            mailBody.Append("Please remember to keep this information on hand for placing future orders.<br/><br/> ");
            mailBody.Append("Now that you have registered with ");
            mailBody.Append(storefrontName);
            mailBody.Append(", you have two options for ordering with us:<br><br>");

            mailBody.Append("<ol>");
            mailBody.Append("<li><p align=\"justify\">Call our friendly customer service representatives at <b>");
            mailBody.Append(storefrontContact);
            mailBody.Append("</b> and they will take your order over the phone or answer any questions that you may have.</p></li>");
            mailBody.Append("<li><p align=\"justify\">Visit us on our website at <a href=\"http://www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("\"> www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("</a>");
            mailBody.Append(" and place your entire order online.</p></li>");
            mailBody.Append("</ol>");
            mailBody.Append("Sincerely, <br/><br/>");
            mailBody.Append(storefrontUrl);
            mailBody.Append("<br/><br/> <br/><br/>");

            mailBody.Append("</p>");
            mailBody.Append("</td>");

            mailBody.Append("<td><img src=\"http://www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("/content/images/email/spacer.gif\" width=\"20\" height=\"0\">");
            mailBody.Append("</td>");
            
            mailBody.Append("</tr>");
            mailBody.Append("</table>");
            mailBody.Append("</td>");
            mailBody.Append("</tr>");
            mailBody.Append("</table>");


            mailBody.Append("</div>");
            mailBody.Append("</body>");
            mailBody.Append("</html>");
            return mailBody.ToString();
        }

        // create password recovery email

        public static string PasswordRecoveryEmail(int customerId,
                                                   string email,
                                                   string password,
                                                   string storefrontName,
                                                   string storefrontContact,
                                                   string storefrontFax,
                                                   string storefrontUrl,
                                                   string storefrontEmailHeader,
                                                   string storefrontEnquiry)
        {
            StringBuilder mailBody = new StringBuilder("");
            mailBody.Append("<html>");

            mailBody.Append("<head>");

            mailBody.Append("<title>");
            mailBody.Append("Here is your login information");
            mailBody.Append("</title>");

            mailBody.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\">");
            mailBody.Append("<style type=\"text/css\"> Body  { font-size : 9pt; font-family:arial,sans-serif; font-weight : normal; font-style : normal; color : ##3F3F3F; }");
            mailBody.Append("p { font-size : 9pt; font-family:arial,sans-serif; font-weight : normal; font-style : normal; color : ##3F3F3F; }");
            mailBody.Append("td { font-size : 9pt; font-family:arial,sans-serif; font-weight : normal; font-style : normal; color : ##3F3F3F; }");

            mailBody.Append("</style>");

            mailBody.Append("</head>");

            mailBody.Append(
               "<body leftmargin=\"0\" topmargin=\"0\" marginwidth=\"0\" marginheight=\"0\" alink=\"2F435E\" vlink=\"61B7E6\">");

            mailBody.Append("<div align=\"center\">");

            mailBody.Append("<table bgcolor=\"#FFFFFF\" width=\"688\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-top:4px #032525 solid;border-bottom:4px #032525 solid;border-left:4px #032525 solid;border-right:4px #032525 solid\">");

            mailBody.Append("<tr>");
            mailBody.Append(" <td colspan=\"2\"><a href=\"\"><img src=\"http://www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("/content/images/email/");
            mailBody.Append(storefrontEmailHeader);
            mailBody.Append(" \"border=\"0\" ></a>");
            mailBody.Append("</td>");
            mailBody.Append("</tr>");

            mailBody.Append("<tr>");
            mailBody.Append("<td valign=\"top\" colspan=\"2\">");
            mailBody.Append("<table bgcolor=\"FFFFFF\" cellpadding=\"0\" cellspacing=\"0\">");
            mailBody.Append("<tr>");
            
            mailBody.Append("<td><img src=\"http://www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("/content/images/email/spacer.gif\" width=\"20\" height=\"0\">");
            mailBody.Append("</td>");
            
            mailBody.Append("<td>");
            mailBody.Append("<br><br>You have requested to have your password to be sent to this email account.");
            mailBody.Append("<br><br>Here is your customer number, email address (log-in) and password for your account:<br/>");

            mailBody.Append("<table style=\"border-top:1px #032525 solid;border-bottom:1px #032525 solid;border-left:1px #032525 solid;border-right:1px #032525 solid\">");
          
            mailBody.Append("<tr>");
            
            mailBody.Append("<td>");
            mailBody.Append("Your customer number is: ");
            mailBody.Append("</td>");
            
            mailBody.Append("<td align=\"right\">");
            mailBody.Append("<b>");
            mailBody.Append(customerId);
            mailBody.Append("</b>");
            mailBody.Append("</td>");
            
            mailBody.Append("</tr>");

            mailBody.Append("<tr>");
            
            mailBody.Append("<td>");
            mailBody.Append("Your email address is: ");
            mailBody.Append("</td>");
            
            mailBody.Append("<td align=\"right\">");
            mailBody.Append("<b>");
            mailBody.Append(email);
            mailBody.Append("</b>");
            mailBody.Append("</td>");
            
            mailBody.Append("</tr>");

            mailBody.Append("<tr>");
            
            mailBody.Append("<td>");
            mailBody.Append("Your password is: ");
            mailBody.Append("</td>");
            
            mailBody.Append("<td align=\"right\">");
            mailBody.Append("<b>");
            mailBody.Append(password);
            mailBody.Append("</b>");
            mailBody.Append("</td>");
            
            mailBody.Append("</tr>");

            mailBody.Append("</table> <br/>");

            mailBody.Append("<br/>Please remember to keep this information on hand for placing future orders.<br/><br/> ");
            mailBody.Append("Now that you have registered with ");
            mailBody.Append(storefrontName);
            mailBody.Append(", you have two options for ordering with us:<br><br>");

            mailBody.Append("<ol>");
            mailBody.Append("<li><p align=\"justify\">Call our friendly customer service representatives at <b>");
            mailBody.Append(storefrontContact);
            mailBody.Append("</b> and they will take your order over the phone or answer any questions that you may have.</p></li>");
            mailBody.Append("<li><p align=\"justify\">Visit us on our website at <a href=\"http://www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("\"> www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("</a>");
            mailBody.Append(" and place your entire order online.</p></li>");
            mailBody.Append("</ol>");
            mailBody.Append("Sincerely, <br/><br/>");
            mailBody.Append(storefrontUrl);
            mailBody.Append("<br/><br/> <br/><br/>");

            mailBody.Append("</p>");
            mailBody.Append("</td>");
            mailBody.Append("<td><img src=\"http://www.");
            mailBody.Append(storefrontUrl);
            mailBody.Append("/content/images/email/spacer.gif\" width=\"20\" height=\"0\">");
            mailBody.Append("</td>");


            mailBody.Append("</tr>");
            mailBody.Append("</table>");
            mailBody.Append("</td>");
            mailBody.Append("</tr>");
            mailBody.Append("</table>");


            mailBody.Append("</div>");
            mailBody.Append("</body>");
            mailBody.Append("</html>");
            return mailBody.ToString();
        }

        // order confirmation email
        public static string OrderCreatedEmail(int orderInvoiceId,
                                               string customerFirstName, 
                                               string shippingFirstName, 
                                               string shippingLastName,
                                               string shippingAddress,
                                               string shippingCity,
                                               string shippingZipCode,
                                               string shippingCountryCode,
                                               string shippingProvinceCode,
                                               Cart cart,
                                               AffiliateInfo affiliateInfo)
        {
            StringBuilder mailBody = new StringBuilder("");
            mailBody.Append("<html>");

            mailBody.Append("<head>");

            mailBody.Append("<title>");
            mailBody.Append("Thank-you for ordering with ");
            mailBody.Append(affiliateInfo.StorefrontUrl);
            mailBody.Append("</title>");

            mailBody.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\">");
            mailBody.Append("<style type=\"text/css\"> Body  { font-size : 9pt; font-family:arial,sans-serif; font-weight : normal; font-style : normal; color : ##3F3F3F; }");
            mailBody.Append("p { font-size : 9pt; font-family:arial,sans-serif; font-weight : normal; font-style : normal; color : ##3F3F3F; }");
            mailBody.Append("td { font-size : 9pt; font-family:arial,sans-serif; font-weight : normal; font-style : normal; color : ##3F3F3F; }");

            mailBody.Append("</style>");

            mailBody.Append("</head>");

            mailBody.Append(
              "<body leftmargin=\"0\" topmargin=\"0\" marginwidth=\"0\" marginheight=\"0\" alink=\"2F435E\" vlink=\"61B7E6\">");

            mailBody.Append("<div align=\"center\">");

            mailBody.Append("<table bgcolor=\"#FFFFFF\" width=\"688\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-top:4px #032525 solid;border-bottom:4px #032525 solid;border-left:4px #032525 solid;border-right:4px #032525 solid\">");

            mailBody.Append("<tr>");
            mailBody.Append(" <td colspan=\"2\"><a href=\"\"><img src=\"http://www.");
            mailBody.Append(affiliateInfo.StorefrontUrl);
            mailBody.Append("/content/images/email/");
            mailBody.Append(affiliateInfo.StorefrontEmailHeader);
            mailBody.Append(" \"border=\"0\" ></a>");
            mailBody.Append("</td>");
            mailBody.Append("</tr>");


            mailBody.Append("<tr height=\"20\">");
              mailBody.Append("<td align=\"left\" >");
              mailBody.Append("&nbsp;&nbsp;&nbsp;&nbsp;Dear: ");
              mailBody.Append(customerFirstName);
              mailBody.Append("</td>");
              mailBody.Append("<td align=\"right\" >");
              mailBody.Append(DateTime.Now.ToString("MMMM, dd, yyyy"));
              mailBody.Append("&nbsp;&nbsp;&nbsp;&nbsp;</td>");
            mailBody.Append("</tr>");

            mailBody.Append("<tr>");
              mailBody.Append(" <td colspan=\"2\" valign=\"top\">");

                mailBody.Append("<table bgcolor=\"FFFFFF\" cellpadding=\"0\" cellspacing=\"0\">");

                mailBody.Append("<tr>");

                mailBody.Append("<td><img src=\"http://www.");
                mailBody.Append(affiliateInfo.StorefrontUrl);
                mailBody.Append("/content/images/email/spacer.gif\" width=\"20\" height=\"0\">");
                mailBody.Append("</td>");
            

                    mailBody.Append("<td>");
                    mailBody.Append("<br><p align=\"justify\"><font style=\"color: 2F435E; font-size: 17px;\"><b>Thank you for placing an order with ");
                    mailBody.Append(affiliateInfo.StorefrontName);
                    mailBody.Append("!</b></font><br/><br/>");
                    mailBody.Append("<b>Confirmation:</b> Your order has been placed and you will be contacted by Customer Service to select a method of payment and complete your order.<br/><br/>");
                    mailBody.Append("* Shipments usually arrive within 8 – 12 business days <br/>");
                    mailBody.Append("* If you have ordered more than 1 product the order may be separated and shipped from different affiliated pharmacies.<br/><br/>");

                    mailBody.Append("Your order number is : <b>");
                    mailBody.Append(orderInvoiceId);
                    mailBody.Append("</b><br><br>");

                    mailBody.Append("<table border=\"0\" width=\"100%\">");
                    mailBody.Append("<tr>");
                    mailBody.Append("<td> </td>");
                    mailBody.Append("</tr>");

                    mailBody.Append("<tr>");
                    mailBody.Append("<td><b>SHIPPING ADDRESS:</b> </td>");
                    mailBody.Append("</tr>");

                    mailBody.Append("<tr>");
                    mailBody.Append(" <td valign=\"top\">");
            
                    mailBody.Append(shippingFirstName);
                    mailBody.Append(" ");
                    mailBody.Append(shippingLastName);
                    mailBody.Append("<br/>");
                
                    mailBody.Append(shippingAddress);
                    mailBody.Append("<br/>");

                    mailBody.Append(shippingCity);
                    mailBody.Append(", ");

                    mailBody.Append(shippingProvinceCode);
                    mailBody.Append(", ");

                    mailBody.Append(shippingCountryCode);
                    mailBody.Append("<br/>");

                    mailBody.Append(shippingZipCode);
                    mailBody.Append("<br/>");
            
                    mailBody.Append("</td>");
                    mailBody.Append("</tr>");
                    mailBody.Append("</table>");

                    mailBody.Append("<br/><font style=\"color: 2F435E;\"><b>Order Summary:</b></font><br> ");
                    mailBody.Append("<table width=\"100%\" bgcolor=\"FFFFFF\" style=\"border-top:1px CED4B4 solid;border-bottom:1px CED4B4 solid;border-left:1px CED4B4 solid;border-right:1px CED4B4 solid\">");

                    mailBody.Append("<tr>");
			        mailBody.Append("<td>&nbsp;</td>");

                    mailBody.Append("<td align=\"left\">");
                    mailBody.Append("<b>Product</b>");
                    mailBody.Append("</td>");
	
			        mailBody.Append("<td align=\"left\">");
                    mailBody.Append("<b>Size and Strength</b>");
                    mailBody.Append("</td>");
                    mailBody.Append("<td align=\"left\">");
                    mailBody.Append("<b>Qty</b>");
                    mailBody.Append("</td>");

                    mailBody.Append("<td align=\"left\">");
                    mailBody.Append("<b>Price</b>");
                    mailBody.Append("</td>");
                    mailBody.Append("</tr>");

                    var prodCount = 1;
                    foreach (var cartItem in cart.CartItems)
                    {
                        mailBody.Append("<tr>");

                        mailBody.Append("<td align= \"left\">");
                        mailBody.Append(prodCount);
                        mailBody.Append("</td>");

                        mailBody.Append("<td align= \"left\">");
                        mailBody.Append(cartItem.ProductCart.ProductSizeGeneric ? "Generic" : "Brand");
                        mailBody.Append(" ");
                        mailBody.Append(cartItem.ProductCart.ProductName);
                        mailBody.Append("</td>");

                        mailBody.Append("<td align= \"left\">");
                        mailBody.Append(cartItem.ProductCart.ProductSizeGeneric ? "<b>Generic</b>" : "<b>Brand</b>");
                        mailBody.Append(" ");
                        mailBody.Append(cartItem.ProductCart.ProductSizeStrength);
                        mailBody.Append(" - ");
                        mailBody.Append(cartItem.ProductCart.ProductSizeQuantity);
                        mailBody.Append("</td>");

                        mailBody.Append("<td align= \"left\">");
                        mailBody.Append(cartItem.Quantity);
                        mailBody.Append("</td>");

                        mailBody.Append("<td align= \"left\">");
                        mailBody.Append(cartItem.ProductCart.ProductSizeStoreFrontPrice.ToString("c"));
                        mailBody.Append("</td>");

                        mailBody.Append("</tr>");

                    }
                         mailBody.Append("</table>");


                        mailBody.Append("<br/><br/>");

                        if (cart.ShippingPrice != 0)
                        {
                            mailBody.Append("Shipping: <b>");
                            mailBody.Append(cart.ShippingPrice.ToString("c"));
                            mailBody.Append("</b><br/>");
                        }

                        if (cart.CouponAmount != 0)
                        {
                            mailBody.Append("Your Coupon discount is: <b>");
                            mailBody.Append((cart.CouponAmount * -1).ToString("c"));
                            mailBody.Append("</b><br/>");
                        }

                        if (cart.CreditApplied != 0)
                        {
                            mailBody.Append("Credit applied to this order is:: <b>");
                            mailBody.Append((cart.CreditApplied * -1).ToString("c"));
                            mailBody.Append("</b><br/>");
                        }

                         mailBody.Append("Your order total comes to : <b>");
                         mailBody.Append(cart.CartNetTotal.ToString("c"));
                         mailBody.Append("</b>");

                        if (cart.CartHasRxProducts())
                        {
                            mailBody.Append("<br><br><b>If your order requires a prescription, please read the following instructions.</b> <br/> <br/>");
                            mailBody.Append("1)	Please remember to fax a copy of your prescription to: <br/> <br/>");

                            mailBody.Append("<b>Fax Line:</b><br>");
                            mailBody.Append(affiliateInfo.StoreFrontFax);
                            mailBody.Append("<br/><br/>");
                            mailBody.Append("*Please be sure to include your order invoice number on all documentation being sent in to us.  If mailing, please remember to use appropriate postage, to ensure its arrival.");
                            mailBody.Append("<br/><br/>");
                            mailBody.Append("2)	If this is a <u><b>refill</b></u> order, you are not required to send a photocopy of the prescription to us, as we already have it saved on file.");
                            mailBody.Append("<br/><br/>");
                            mailBody.Append("*Please make sure to double check if you have refills remaining on file, to avoid any delay on the order.");
                            mailBody.Append("<br/><br/>");
                            mailBody.Append("3)	Once you have received your first order, please mail the original prescription in the return envelope issued in the package to the address above.");

                        }

                        mailBody.Append("<br/><br/>");
                        mailBody.Append("If you require any further assistance, please call us toll free at <font color=\"2F435E\"><b>");
            
                        mailBody.Append(affiliateInfo.StorefrontContact);
                        mailBody.Append("</b></font>.");

                        mailBody.Append("<br/><br/>");

                        mailBody.Append("Sincerely,"); 
                        mailBody.Append("<br/><br/>");
                        mailBody.Append(affiliateInfo.StorefrontUrl);
                        mailBody.Append("<br/><br/>");
                        mailBody.Append("</td>");


                        mailBody.Append("<td><img src=\"http://www.");
                        mailBody.Append(affiliateInfo.StorefrontUrl);

                        mailBody.Append("/content/images/email/spacer.gif\" width=\"20\" height=\"0\">");
                        mailBody.Append("</td>");

                        mailBody.Append("</tr>");
                        mailBody.Append("</table>");
            
                        mailBody.Append("</td>");
                        mailBody.Append("</tr>");
                        mailBody.Append("</table>");

                        mailBody.Append("</div>");

                        mailBody.Append("</body>");
                        mailBody.Append("</html>");

                return mailBody.ToString();
        }
    }
}
