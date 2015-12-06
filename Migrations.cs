using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using System;
using System.Data;

namespace Cascade.WebShop
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("ProductRecord", table => table
                .ContentPartRecord()
                .Column<decimal>("UnitPrice")
                .Column<string>("Sku", column => column.WithLength(50))
                .Column<int>("InStock")
                .Column<int>("NumberSold")
                .Column<bool>("CanReorder")
                .Column<int>("ReorderLevel")
                .Column<bool>("IsShippable", column => column.WithDefault(true))
                );

            ContentDefinitionManager.AlterPartDefinition("ProductPart", part => part
                .Attachable());

            ContentDefinitionManager.AlterTypeDefinition("ShoppingCartWidget", type => type
                .WithPart("ShoppingCartWidgetPart")
                .WithPart("WidgetPart")
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
                .ContentPartRecord()
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
                .ContentPartRecord()
                .Column<int>("OrderRecord_Id", c => c.NotNull())
                .Column<int>("ProductPartRecord_Id", c => c.NotNull())
                .Column<int>("Quantity", c => c.NotNull())
                .Column<decimal>("UnitPrice", c => c.NotNull())
                .Column<decimal>("GSTRate", c => c.NotNull())
                .Column<Decimal>("UnitGST", c => c.Nullable())
                .Column<Decimal>("GST", c => c.Nullable())
                .Column<Decimal>("SubTotal", c => c.Nullable())
                .Column<Decimal>("Total", c => c.Nullable())
                .Column<string>("Sku")
                .Column<string>("Description")
            );


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
                .Column<int>("ShippingProductRecord_id", column => column.Nullable())
            );

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

            SchemaBuilder.CreateTable("ShippingProductRecord", table => table
                .ContentPartRecord()
                .Column<string>("Title", c => c.WithLength((100)))
                .Column<string>("Description")
                );

            ContentDefinitionManager.AlterPartDefinition("ShippingProductPart", part =>
                part.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("ShippingProduct", type => type
                .WithPart("CommonPart")
                .WithPart("ProductPart")
                .WithPart("ShippingProductPart")
                .Creatable()
                );

            return 1;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable("ProductRecord", t => t
                .AddColumn<bool>("UseStockControl"));
            return 2;
        }
        public int UpdateFrom2()
        {
            SchemaBuilder.DropTable("OrderRecord");

            SchemaBuilder.CreateTable("OrderRecord", table => table
                .ContentPartRecord()
                .Column<int>("CustomerId", c => c.NotNull())
                .Column<DateTime>("CreatedAt", c => c.Nullable())
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
                .Column<string>("RawDetails", c => c.Unlimited())
                );

            ContentDefinitionManager.AlterPartDefinition("OrderRecordPart", part => part
                .Attachable(false)
            );

            ContentDefinitionManager.AlterTypeDefinition("Order", builder => builder
                .WithPart("OrderRecordPart")
            );

            SchemaBuilder.DropTable("OrderDetailRecord");

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable("AddressRecord", table => table
                .AddColumn<int>("OrderId")
           );
            return 4;
        }
    }

    //[OrchardFeature("Booking", FeatureName = "Cascade.WebShop.Feature1")]
    //public class MyFeatureMigrations : DataMigrationImpl
    //{
    //    public int Create()
    //    {
    //        SchemaBuilder.CreateTable("Booking", table => table
    //            .ContentPartRecord()
    //            .Column<string>("PreferredDay")
    //            .Column<string>("PreferredTime")
    //            .Column<string>("Notes")
    //            .Column<string>("Name")
    //            .Column<string>("Address")
    //            .Column<string>("City")
    //            .Column<string>("State")
    //            .Column<string>("Postcode")
    //            .Column<string>("CountryCode", c => c.WithLength(2))
    //        );

    //        ContentDefinitionManager.AlterTypeDefinition("Boo")
    //        ContentDefinitionManager.AlterPartDefinition("BookingPart", builder => builder
    //            .Attachable(false)
    //            );

    //        return 1;
    //    }
    //}
}
