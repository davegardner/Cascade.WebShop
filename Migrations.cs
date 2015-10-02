using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using System;
using System.Data;

namespace Cascade.WebShop
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("ProductRecord", table => table

                // The following method will create an "Id" column for us and set it as the primary key for the table
                .ContentPartRecord()

                // Create a column named "UnitPrice" of type "decimal"
                .Column<decimal>("UnitPrice")

                // Create the "Sku" column and specify a maximum length of 50 characters
                .Column<string>("Sku", column => column.WithLength(50))

                .Column<int>("InStock")

                .Column<int>("NumberSold")

                .Column<bool>("CanReorder")

                .Column<int>("ReorderLevel")
                );

            // Create (or alter) a part called "ProductPart" and configure it to be "attachable".
            ContentDefinitionManager.AlterPartDefinition("ProductPart", part => part
                .Attachable());

            // Define a new content type called "ShoppingCartWidget"
            ContentDefinitionManager.AlterTypeDefinition("ShoppingCartWidget", type => type

                // Attach the "ShoppingCartWidgetPart"
                .WithPart("ShoppingCartWidgetPart")

                // In order to turn this content type into a widget, it needs the WidgetPart
                .WithPart("WidgetPart")

                // It also needs a setting called "Stereotype" to be set to "Widget"
                .WithSetting("Stereotype", "Widget")

                .WithPart("CommonPart")

                );

            SchemaBuilder.CreateTable("CustomerRecord", table => table
                .ContentPartRecord()
                .Column<string>("FirstName", c => c.WithLength(50))
                .Column<string>("LastName", c => c.WithLength(50))
                .Column<string>("Title", c => c.WithLength(10))
                .Column<DateTime>("CreatedUtc")
                .Column<bool>("SubscribeToMailingList")
                .Column<bool>("AgreeTermsAndConditions")
                .Column<bool>("WelcomeEmailPending")
                .Column<bool>("ReceivePost")
                );

            SchemaBuilder.CreateTable("AddressRecord", table => table
                .ContentPartRecord()
                .Column<int>("CustomerId")
                .Column<string>("Type", c => c.WithLength(50))
                .Column<string>("Name")
                .Column<string>("Address")
                .Column<string>("City")
                .Column<string>("State")
                .Column<string>("Postcode")
                .Column<string>("CountryCode", c => c.WithLength(2))
                );

            ContentDefinitionManager.AlterPartDefinition("CustomerPart", part => part
                .Attachable(false)
                );

            ContentDefinitionManager.AlterTypeDefinition("Customer", type => type
                .WithPart("CustomerPart")
                .WithPart("UserPart")
                .WithPart("UserRolesPart")
                );

            ContentDefinitionManager.AlterPartDefinition("AddressPart", part => part
                .Attachable(false)
                );

            ContentDefinitionManager.AlterTypeDefinition("Address", type => type
                .WithPart("CommonPart")
                .WithPart("AddressPart")
                );


            SchemaBuilder.CreateTable("OrderRecord", t => t
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("CustomerId", c => c.NotNull())
                .Column<DateTime>("CreatedAt", c => c.NotNull())
                .Column<decimal>("SubTotal", c => c.NotNull())
                .Column<decimal>("GST", c => c.NotNull())
                .Column<string>("Status", c => c.WithLength(50).NotNull())
                .Column<string>("PaymentServiceProviderResponse", c => c.WithLength(null))
                .Column<string>("PaymentReference", c => c.WithLength(50))
                .Column<DateTime>("PaidAt", c => c.Nullable())
                .Column<DateTime>("CompletedAt", c => c.Nullable())
                .Column<DateTime>("CancelledAt", c => c.Nullable())
                .Column<decimal>("Total", c => c.Nullable())
                .Column<string>("Number", c => c.Nullable())
                );

            SchemaBuilder.CreateTable("OrderDetailRecord", t => t
                .Column<int>("Id", c => c.PrimaryKey().Identity())
                .Column<int>("OrderRecord_Id", c => c.NotNull())
                .Column<int>("ProductPartRecord_Id", c => c.NotNull())
                .Column<int>("Quantity", c => c.NotNull())
                .Column<decimal>("UnitPrice", c => c.NotNull())
                .Column<decimal>("GSTRate", c => c.NotNull())
                .Column<Decimal>("UnitGST", c => c.Nullable())
                .Column<Decimal>("GST", c => c.Nullable())
                .Column<Decimal>("SubTotal", c => c.Nullable())
                .Column<Decimal>("Total", c => c.Nullable())
                );


            //SchemaBuilder.CreateForeignKey("Order_Customer", "OrderRecord", new[] { "Id" }, "CustomerPartRecord", new[] { "Id" });
            //SchemaBuilder.CreateForeignKey("OrderDetail_Order", "OrderDetailRecord", new[] { "OrderRecord_Id" }, "OrderRecord", new[] { "Id" });
            //SchemaBuilder.CreateForeignKey("OrderDetail_Product", "OrderDetailRecord", new[] { "ProductPartRecord_Id" }, "ProductPartRecord", new[] { "Id" });

            SchemaBuilder.CreateTable("WebShopSettingsRecord", table => table
                .ContentPartRecord()
                .Column<string>("AdministratorEmailAddress")
                .Column<string>("ContinueShoppingUrl")
                .Column<bool>("ShowSubscribeToMailingList")
                .Column<bool>("ShowTermsAndConditions")
                .Column<bool>("SendWelcomeEmail")
                .Column<string>("TermsAndConditionsUrl")
                .Column<string>("PrivacyUrl")
                .Column<string>("WelcomeSubject")
                .Column<string>("WelcomeBodyTemplate", c => c.Unlimited())
                .Column<string>("UnsubscribeEmail")
                .Column<bool>("UseMailChimp")
                .Column<string>("MailChimpApiKey")
                .Column<string>("MailChimpListName")
                .Column<string>("MailChimpGroupName")
                .Column<string>("MailChimpGroupValue")
            );

            // Creating table TransactionRecord
            SchemaBuilder.CreateTable("TransactionRecord", table => table
                .Column("Id", DbType.Int32, column => column.PrimaryKey().Identity())
                .Column("OrderRecord_Id", DbType.Int32)
                .Column("Method", DbType.String)
                .Column("Token", DbType.String)
                .Column("Ack", DbType.String)
                .Column("PayerId", DbType.String)
                .Column("RequestTransactionId", DbType.String)
                .Column("RequestSecureMerchantAccountId", DbType.String)
                .Column("RequestAck", DbType.String)
                .Column("DateTime", DbType.DateTime)
                .Column("Timestamp", DbType.DateTime)
                .Column("ErrorCodes", DbType.String)
                .Column("ShortMessages", DbType.String)
                .Column("LongMessages", DbType.String)
                .Column("CorrelationId", DbType.String)
                );

            return 1;
        }

        //public int UpdateFrom1()
        //{
        //    // Creating table ShippingProductRecord
        //    SchemaBuilder.CreateTable("ShippingProductRecord", table => table
        //        .ContentPartRecord()
        //        .Column<string>("Title", c => c.WithLength((100)))
        //        .Column<string>("Description")
        //        .Column<decimal>("PriceIncGst", c => c.NotNull())
        //        .Column<decimal>("GstRate", c => c.NotNull())
        //        );

        //    SchemaBuilder.AlterTable("WebShopSettingsRecord", table =>
        //    {
        //        table.AddColumn<int>("ShippingProductRecord_id", column => column.Nullable());
        //    });

        //    return 2;
        //}

        //public int UpdateFrom2()
        //{
        //    // Creating table ShippingProductRecord
        //    SchemaBuilder.CreateTable("ShippingProductRecord", table => table
        //        .ContentPartRecord()
        //        .Column<string>("Title", c => c.WithLength((100)))
        //        .Column<string>("Description")
        //        .Column<decimal>("PriceIncGst", c => c.NotNull())
        //        .Column<decimal>("GstRate", c => c.NotNull())
        //        );
        //    return 3;
        //}

        //public int UpdateFrom3()
        //{
        //    SchemaBuilder.AlterTable("CustomerRecord", table =>
        //    {
        //        table.AddColumn<bool>("ReceivePost", c => c.WithDefault(false));
        //    });
        //    return 4;
        //}
        //public int UpdateFrom4()
        //{
        //    SchemaBuilder.AlterTable("WebShopSettingsRecord", table =>
        //    {
        //        table.AddColumn<bool>("UseMailChimp");
        //        table.AddColumn<string>("MailChimpApiKey");
        //        table.AddColumn<string>("MailChimpListName");
        //        table.AddColumn<string>("MailChimpGroupName");
        //        table.AddColumn<string>("MailChimpGroupValue");
        //    });
        //    return 5;
        //}

        public int UpdateFrom5()
        {
            // Creating table ShippingProductRecord
            SchemaBuilder.CreateTable("ShippingProductRecord", table => table
                .ContentPartRecord()
                .Column<string>("Title", c => c.WithLength((100)))
                .Column<string>("Description")
                //.Column<decimal>("PriceIncGst", c => c.NotNull())
                //.Column<decimal>("GstRate", c => c.NotNull())
                );

            SchemaBuilder.AlterTable("WebShopSettingsRecord", table =>
            {
                table.AddColumn<int>("ShippingProductRecord_id", column => column.Nullable());
            });

            ContentDefinitionManager.AlterPartDefinition("ShippingProductPart", part =>
                part.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("ShippingProduct", type => type
                .WithPart("CommonPart")
                .WithPart("ProductPart")
                .WithPart("ShippingProductPart")
                .Creatable()
                );

            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable("ProductRecord", table =>
                {
                    table.AddColumn<bool>("IsShippable", column => column.WithDefault(true));
                });

            return 7;
        }

        public int UpdateFrom7()
        {
            SchemaBuilder.AlterTable("OrderDetailRecord", table =>
            {
                table.AddColumn<string>("Sku");
                table.AddColumn<string>("Description");
            });

            return 8;
        }
    }
}
