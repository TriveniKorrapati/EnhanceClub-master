using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EnhanceClub.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // route for terms and Conditions
            routes.MapRoute("telehealth-consent", "telehealth-consent", new { Controller = "Home", action = "telehealth-consent" });

            // route for blog page
            routes.MapRoute("blog-route", "blog", new { Controller = "Blog", action = "Index" });

            // route for blog post url         
            routes.MapRoute("blog-route1", "blog/{blogUrl}", new { Controller = "Blog", action = "BlogSelector", blogUrl = UrlParameter.Optional });

            
            // route for faq
            routes.MapRoute("faq-route", "faq", new { Controller = "Home", action = "Faq" });

            
            // route for about us page
            routes.MapRoute("about-us-route", "about", new { Controller = "Home", action = "About" });

            // route for how to order
            routes.MapRoute("HowItWorks-route", "how-it-works", new { Controller = "Home", action = "how-it-works"});

            // route for how get in touch
            routes.MapRoute("GetInTouch-route", "get-in-touch", new { Controller = "Home", action = "get-in-touch" });

            // route for thank you
            routes.MapRoute("thank-you-route", "thank-you", new { Controller = "Customer", action = "thank-you" });

            // route for Shipping and returns
            routes.MapRoute("ShippingAndReturns-route", "shipping-and-returns", new { Controller = "Home", action = "shipping-and-returns" });

            // route for terms and Conditions
            routes.MapRoute("Terms-route", "terms-and-conditions", new { Controller = "Home", action = "terms-and-conditions" });

            // route for terms and Conditions
            routes.MapRoute("Privacy-route", "privacy-policy", new { Controller = "Home", action = "privacy-policy" });
            

            routes.MapRoute("customer-route1", "login", new { Controller = "Customer",action = "login" });

            routes.MapRoute("customer-route2","account",new {Controller = "Customer", action = "account"});
            
            routes.MapRoute("customer-route-3", "get-started", new { Controller = "Customer", action = "get-started" });

            routes.MapRoute("customer-route-4", "get-started-step-2", new { Controller = "Customer", action = "get-started-step-2" });

            routes.MapRoute("customer-route-5", "get-started-step-3", new { Controller = "Customer", action = "get-started-step-3" });

            routes.MapRoute("customer-route-6", "forgot-password", new { Controller = "Customer", action = "forgot-password" });

            routes.MapRoute("customer-route-7", "reset-password", new { Controller = "Customer", action = "reset-password" });

            routes.MapRoute("customer-route-8", "myaccount/account-info/{partialprofile}", new { Controller = "Customer", action = "account-info", partialprofile = UrlParameter.Optional });

            routes.MapRoute("customer-route-9", "dashboard", new
            {
                Controller = "Customer",
                action = "dashboard"
            });

            routes.MapRoute("customer-myaccount-route2", "myaccount", new
            {
                Controller = "Customer",
                action = "myaccount"
            });

            routes.MapRoute("customer-route-10", "id-documents", new { Controller = "Customer", action = "id-documents" });
            

            routes.MapRoute("customer-invoice-route", "viewinvoice/{orderId}", new
            {
                controller = "Customer",
                action = "OrderInvoicePdf",
                orderId = UrlParameter.Optional,

            });

            routes.MapRoute("general-product", "products", new
            {
                controller = "Product",
                action = "Products",

            });
            
            //routes.MapRoute(
            //    name: "ComingSoon",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Marketing", action = "ComingSoon", id = UrlParameter.Optional }
            //);

            // This route is used for product detail page 
            routes.MapRoute("product-search-name", "products/{searchTerm}", new
            {
                Controller = "Product",
                action = "ProductDetail",
                searchTerm = UrlParameter.Optional

            });

            // route for order confirm
            routes.MapRoute("order-confirm", "orderconfirm/{orderId}/sale/{orderTotal}", new
            {
                Controller = "Order",
                action = "OrderConfirm",
                orderId = UrlParameter.Optional,
                orderTotal = UrlParameter.Optional,                
            });
            //Route to Upload Prescription/ID
            routes.MapRoute("customer-upload", "upload/{orderId}", new
            {
                controller = "Order",
                action = "UploadDocument",
                orderId = UrlParameter.Optional
            });
            //Route to Upload Prescription/ID
            routes.MapRoute("RequireRx", "RequirePrescription/{orderId}", new
            {
                controller = "Order",
                action = "RequirePrescription",
                orderId = UrlParameter.Optional
            });
            //Route to Upload Prescription/ID
            routes.MapRoute("upload-document-success", "upload-document-success", new
            {
                controller = "Order",
                action = "upload-document-success",
                
            });

            routes.MapRoute("prescription-options", "prescription-options/{orderId}", new
            {
                controller = "Order",
                action = "PrescriptionOptions",
                orderId = UrlParameter.Optional

            });
           
            routes.MapRoute("customer-patient-profile", "myaccount/edit-patient", 
                new {
                    Controller = "Customer",
                    action = "EditPatientProfile",
                    partialprofile = UrlParameter.Optional
                });

            routes.MapRoute("rx-review-answer", "reviewanswer/{orderId}", new
            {
                controller = "Order",
                action = "ReviewAnswerGet",
                orderId = UrlParameter.Optional
            });

            routes.MapRoute("rx-review-answer-1", "requireprescriptionreview/{orderId}", new
            {
                controller = "Order",
                action = "RequirePrescriptionReviewAnswers",
                orderId = UrlParameter.Optional
            });

            routes.MapRoute("delete-address", "customer/deleteaddress/{shippingAddressFk}",
                new
                {
                    controller = "Customer",
                    action = "DeleteShippingAddress",
                    shippingAddressFk = UrlParameter.Optional
                });

            routes.MapRoute("refill-order", "refill-order/{shippingInvoiceId}", new
            {
                controller = "Order",
                action = "RefillOrder",
                shippingInvoiceId = UrlParameter.Optional

            });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{searchTerm}",
                defaults: new { controller = "Home", action = "Index", searchTerm = UrlParameter.Optional }
            );

           


        }
    }
}
