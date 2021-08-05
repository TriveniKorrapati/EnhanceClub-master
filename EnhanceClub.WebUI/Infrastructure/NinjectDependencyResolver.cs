using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Concrete;
using EnhanceClub.Domain.Entities;
using Moq;
using Ninject;

// Created by Rajiv S : 26 March 2020
namespace EnhanceClub.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            // put bindings here
            
            // mockMenuTabs represents advanced menu which can have more than one item in a tab
            Mock<IMenuTabRepository> mockMenuTabs = new Mock<IMenuTabRepository>();

            mockMenuTabs.Setup(m => m.MenuTabs).Returns(new List<MenuTab>
            {
                //Commented Home menu on 13 May 2020
                 new MenuTab
                 {
                     TabItems = new List<MenuItem> 
                     { 
                         new MenuItem { LinkController = "Home" , LinkAction = "index", LinkText = "Home" , HasChildMenu =false,HasChildTab=false  }
                     }
                     
                 },

                  new MenuTab
                 {
                    TabHeader = "Shop All" , TabUrl = "/prescription", TabType="table", TabItems = new List<MenuItem> 
                     { 
                         //Added Products under Product menu on 13 May 2020
                       
                          new MenuItem { LinkController = "Product" , LinkAction = "ProductDetail", LinkText = "Amlodipine", ProductSearchTerm="amlodipine" , ResetParam = true,ProductType="rx"},
                         new MenuItem { LinkController = "Product" , LinkAction = "ProductDetail", LinkText = "Atenol", ProductSearchTerm="atenol" , ResetParam = true,ProductType="rx"},
                         new MenuItem { LinkController = "Product" , LinkAction = "ProductDetail", LinkText = "Sildenafil" , ResetParam = true, ProductSearchTerm="sildenafil",ProductType="rx"},
                         new MenuItem { LinkController = "Product" , LinkAction = "ProductDetail", LinkText = "Simvastatin", ProductSearchTerm="simvastatin" , ResetParam = true,ProductType="rx"},
                        

                      
                         new MenuItem { LinkController = "Product" , LinkAction = "ProductDetail", LinkText = "COLD-FX", ProductSearchTerm="cold-fx" , ResetParam = true,ProductType="otc"},
                         new MenuItem { LinkController = "Product" , LinkAction = "ProductDetail", LinkText = "COLDSORE-FX", ProductSearchTerm="coldsore-fx" , ResetParam = true,ProductType="otc"},
                         new MenuItem { LinkController = "Product" , LinkAction = "ProductDetail", LinkText = "Systane Ultra" , ProductSearchTerm="systane-ultra", ResetParam = true,ProductType="otc"},
                         new MenuItem { LinkController = "Product" , LinkAction = "ProductDetail", LinkText = "Nicorette Gum" , ProductSearchTerm="nicorette-gum", ResetParam = true,ProductType="otc"},
                        // new MenuItem { LinkController = "Product" , LinkAction = "pet", LinkText = "Pet Medications" , ResetParam = true}
                     }
                     
                 },

                 new MenuTab
                 {
                     TabItems = new List<MenuItem> 
                     { 
                         new MenuItem { LinkController = "Home" , LinkAction = "index", LinkText = "Online Doctor" , HasChildMenu =false,HasChildTab=false  }
                     }
                     
                 },

                  new MenuTab
                 {
                     TabItems = new List<MenuItem> 
                     { 
                         new MenuItem { LinkController = "Home" , LinkAction = "index", LinkText = "Prescriptions" , HasChildMenu =false,HasChildTab=false  }
                     }
                     
                 },

                 new MenuTab
                 {
                     TabItems = new List<MenuItem> 
                     { 
                           new MenuItem { LinkController = "Home" , LinkAction = "index", LinkText = "Advice" , HasChildMenu =false,HasChildTab=false }
                     }
                     
                 },

                 new MenuTab
                 {
                     TabItems = new List<MenuItem> 
                     { 
                           new MenuItem { LinkController = "Home" , LinkAction = "about-us", LinkText = "Info" , HasChildMenu =false,HasChildTab=false }
                     }
                     
                 },
                  new MenuTab
                 {
                     TabItems = new List<MenuItem> 
                     { 
                           new MenuItem { LinkController = "Home" , LinkAction = "howtoorder", LinkText = "How To Order" , HasChildMenu =false,HasChildTab=false }
                     }
                     
                 },
                 new MenuTab
                 {
                     TabItems = new List<MenuItem> 
                     { 
                           new MenuItem { LinkController = "Home" , LinkAction = "faq", LinkText = "FAQ" , HasChildMenu =false,HasChildTab=false }
                     }
                     
                 },
                 new MenuTab
                 {
                     TabItems = new List<MenuItem> 
                     { 
                           new MenuItem { LinkController = "Home" , LinkAction = "drug-safety", LinkText = "Drug Safety" , HasChildMenu =false,HasChildTab=false }
                     }
                     
                 },
                 new MenuTab
                 {
                     TabItems = new List<MenuItem> 
                     { 
                           new MenuItem { LinkController = "Home" , LinkAction = "contact", LinkText = "Contact us" , HasChildMenu =false,HasChildTab=false }
                     }
                     
                 },
                 //  new MenuTab
                 //{
                 //    TabItems = new List<MenuItem> 
                 //    { 
                 //           new MenuItem { LinkController = "home" , LinkAction = "contact",  LinkText = "<p>Contact Us</p>" , ResetParam = false}
                 //    }
                     
                 //}
            });

            // mockMenuItems represents menu where every menu tab can be single item
            Mock<IMenuItemRepository> mockMenuItem = new Mock<IMenuItemRepository>();

       Mock<IFeaturedProductRepository> mockFeaturedProduct = new Mock<IFeaturedProductRepository>();

            mockFeaturedProduct.Setup(m => m.FeaturedProducts).Returns(new List<FeaturedProduct>
            {
                new FeaturedProduct { FeaturedProductName = "Cialis", ProductUrl = "products/cialis", Category = "1", ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Viagra", ProductUrl = "products/viagra", Category = "1" , ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Symbicort", ProductUrl = "products/symbicort-inhaler" , Category = "1", ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Propecia" , ProductUrl = "products/propecia", Category = "1" ,ProductType = "Rx" },
                new FeaturedProduct { FeaturedProductName = "Entocort", ProductUrl = "products/entocort", Category = "1", ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Advair Diskus" , ProductUrl = "products/advair-diskus" , Category = "1", ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Zetia", ProductUrl = "products/zetia", Category = "1" , ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Celebrex", ProductUrl = "products/celebrex", Category = "1" , ProductType = "Rx"},
                
                new FeaturedProduct { FeaturedProductName = "Qualaquin", ProductUrl = "products/qualaquin", Category = "2", ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Spiriva", ProductUrl = "products/spiriva", Category = "2" , ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Flovent Inhaler", ProductUrl = "products/flovent-inhaler" ,Category = "2", ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Premarin" , ProductUrl = "products/premarin" ,Category = "2", ProductType = "Rx" },
                new FeaturedProduct { FeaturedProductName = "Cymbalta", ProductUrl = "products/cymbalta" , Category = "2" , ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Abilify Tablet" , ProductUrl = "products/abilify-tablet" ,Category = "2" , ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Asacol", ProductUrl = "products/asacol" , Category = "2" , ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Flomax", ProductUrl = "products/flomax" ,Category = "2" , ProductType = "Rx"},


                new FeaturedProduct { FeaturedProductName = "Dristan Nasal", ProductUrl = "search/dristan", Category = "1", ProductType = "Otc"},
                new FeaturedProduct { FeaturedProductName = "Nicorette Gum" , ProductUrl = "search/nicorette" , Category = "1", ProductType = "Otc"},
                new FeaturedProduct { FeaturedProductName = "Preparation H", ProductUrl = "search/preparation" , Category = "1", ProductType = "Otc"},
                new FeaturedProduct { FeaturedProductName = "Otrivin", ProductUrl = "search/otrivin", Category = "1" , ProductType = "Otc"},

                new FeaturedProduct { FeaturedProductName = "Ezo Denture", ProductUrl = "products/ezo-denture-cushions", Category = "1" , ProductType = "Otc"},
                new FeaturedProduct { FeaturedProductName = "Lozenges", ProductUrl = "search/thrive", Category = "1" , ProductType = "Otc"},
                new FeaturedProduct { FeaturedProductName = "Anthelios" , ProductUrl = "search/anthelios", Category = "1" ,ProductType = "Otc" },
                new FeaturedProduct { FeaturedProductName = "Senokot", ProductUrl = "search/senokot", Category = "1", ProductType = "Otc"},


                new FeaturedProduct { FeaturedProductName = "Vetmedin", ProductUrl = "search/vetmedin", Category = "1" , ProductType = "Pet"},
                new FeaturedProduct { FeaturedProductName = "Revolution", ProductUrl = "search/revolution", Category = "1" , ProductType = "Pet"},
                new FeaturedProduct { FeaturedProductName = "Frontline Plus" , ProductUrl = "search/frontline", Category = "1" ,ProductType = "Pet" },
                new FeaturedProduct { FeaturedProductName = "Nuheart", ProductUrl = "search/nuheart", Category = "1", ProductType = "Pet"},
                
                new FeaturedProduct { FeaturedProductName = "Previcox", ProductUrl = "search/previcox", Category = "1" , ProductType = "Pet"},
                new FeaturedProduct { FeaturedProductName = "Anipryl", ProductUrl = "products/anipryl", Category = "1" , ProductType = "Pet"},
                new FeaturedProduct { FeaturedProductName = "Insulin" , ProductUrl = "search/easyTouch", Category = "1" ,ProductType = "Pet" },
                new FeaturedProduct { FeaturedProductName = "Comfortis", ProductUrl = "search/comfortis", Category = "1", ProductType = "Pet"}

            });

            //Mock<IFeaturedProductRepository> mockTopProduct = new Mock<IFeaturedProductRepository>();

            // create for Top product list
            mockFeaturedProduct.Setup(m => m.TopProducts).Returns(new List<FeaturedProduct>
            {
                new FeaturedProduct { FeaturedProductName = "Cialis", ProductUrl = "products/cialis", Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Viagra", ProductUrl = "products/viagra", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Taytulla", ProductUrl = "products/taytulla" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Propecia" , ProductUrl = "products/propecia", Category = "1" ,ProductType = "" },
                new FeaturedProduct { FeaturedProductName = "Entocort", ProductUrl = "products/entocort", Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Advair Diskus" , ProductUrl = "products/advair-diskus" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Celebrex", ProductUrl = "products/celebrex", Category = "1" , ProductType = "Rx"},
                new FeaturedProduct { FeaturedProductName = "Symbicort", ProductUrl = "products/symbicort-inhaler" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Saxenda", ProductUrl = "products/saxenda" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Eldoquin Forte", ProductUrl = "products/saxenda" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Pristiq", ProductUrl = "products/pristiq" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Alinia", ProductUrl = "products/alinia" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Lustra Cream", ProductUrl = "products/lustra-cream" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Movantik", ProductUrl = "products/movantik" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Comfortis", ProductUrl = "products/comfortis", Category = "1", ProductType = "Pet"},
                new FeaturedProduct { FeaturedProductName = "Thyroid", ProductUrl = "products/thyroid" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Anthelios" , ProductUrl = "search/anthelios", Category = "1" ,ProductType = "" },
                new FeaturedProduct { FeaturedProductName = "Canthacur" , ProductUrl = "products/canthacur", Category = "1" ,ProductType = "" },
                new FeaturedProduct { FeaturedProductName = "Entresto" , ProductUrl = "products/entresto", Category = "1" ,ProductType = "" },
                new FeaturedProduct { FeaturedProductName = "Edex" , ProductUrl = "products/edex", Category = "1" ,ProductType = "" },
                new FeaturedProduct { FeaturedProductName = "Zetia", ProductUrl = "products/zetia", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Jardiance", ProductUrl = "products/jardiance", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Cosopt", ProductUrl = "products/cosopt", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Livalo", ProductUrl = "products/livalo", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Milbemax", ProductUrl = "products/milbemax", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Nuheart", ProductUrl = "products/nuheart-heartworm-tablets", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Vagifem", ProductUrl = "products/vagifem", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Efudex Cream", ProductUrl = "products/efudex-cream", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Epilyt Lotion", ProductUrl = "products/epilyt.lotion", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Nicorette Gum", ProductUrl = "products/nicorette-gum-2mg", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Parnate", ProductUrl = "products/parnate", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Sentinel", ProductUrl = "products/sentinel-spectrum", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Apriso", ProductUrl = "products/apriso", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Broadline", ProductUrl = "products/broadline", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Cromolyn", ProductUrl = "products/cromolyn-nebules", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Elmiron", ProductUrl = "products/elmiron", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Lialda", ProductUrl = "products/lialda", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Myoflex", ProductUrl = "products/myoflex-cream", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Retin", ProductUrl = "products/retin-a-micro-gel", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Vichy", ProductUrl = "products/vichy-capital-soleil", Category = "1" , ProductType = ""},

            });

            // create for Popular product list
            mockFeaturedProduct.Setup(m => m.PopularProducts).Returns(new List<FeaturedProduct>
            {
                new FeaturedProduct { FeaturedProductName = "Cialis", ProductUrl = "products/cialis", Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Viagra", ProductUrl = "products/viagra", Category = "1" , ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Advair" , ProductUrl = "products/advair-diskus" , Category = "1", ProductType = ""},
                new FeaturedProduct { FeaturedProductName = "Levitra", ProductUrl = "products/levitra", Category = "1" , ProductType = ""},
            
            });

            
            kernel.Bind<IMenuTabRepository>().ToConstant(mockMenuTabs.Object);

            kernel.Bind<IMenuItemRepository>().ToConstant(mockMenuItem.Object);

            kernel.Bind<IFeaturedProductRepository>().ToConstant(mockFeaturedProduct.Object);

            kernel.Bind<ICustomerRepository>().To<CustomerRepositorySql>();
            
            kernel.Bind<IAuthProvider>().To<CognitoAuthProvider>();

            kernel.Bind<IAdminRepository>().To<AdminRepositorySql>();

            kernel.Bind<IStorefrontRepository>().To<StorefrontRepositorySql>();

            kernel.Bind<IProductRepository>().To<ProductRepositorySql>();

            kernel.Bind<IOrderProcessor>().To<OrderRepositorySql>();

            EmailSettings emailSettings = new EmailSettings();

            kernel.Bind<IEmailSender>().To<EmailSender>().WithConstructorArgument("settings", emailSettings);
        }


    }
}