using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cascade.WebShop.Settings
{
    public class ProductSettingsHandler : ContentDefinitionEditorEventsBase
    {
        public override IEnumerable<TemplateViewModel> TypePartEditor(
        ContentTypePartDefinition definition)
        {

            if (definition.PartDefinition.Name != "ProductPart") yield break;
            var model = definition.Settings.GetModel<ProductSettings>();

            model.AvailableModes = Enum.GetValues(typeof(ProductMode))
                .Cast<int>()
                .Select(i =>
                    new
                    {
                        Text = Enum.GetName(typeof(ProductMode), i),
                        Value = i
                    });

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(
            ContentTypePartDefinitionBuilder builder,
            IUpdateModel updateModel)
        {

            if (builder.Name != "ProductPart") yield break;

            var model = new ProductSettings();
            updateModel.TryUpdateModel(model, "ProductSettings", null, null);
            builder.WithSetting("ProductSettings.Mode",
                ((int)model.Mode).ToString());
            yield return DefinitionTemplate(model);
        }
    }
}