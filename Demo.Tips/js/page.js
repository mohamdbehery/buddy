$(document).ready(() => {
    var isPublished = window.location.href.indexOf('github') > 0 ? true : false;
    var url = (isPublished ? "/buddy/Demo.Tips" : "") + "/review-source.xml";
  
    $.ajax({
        type: "GET",
        url: url,
        dataType: "xml",
        success: (xml) => {
            if (xml.children[0]) {
                BindData(xml.children[0]);
            }
        },
        error: (data) => {
            console.log(data);
        }
    });

    var BindData = (data) => {
        var sectionTemplate = $('#section-template').html();
        var itemTemplate = $('#item-template').html();

        var filledSections = '';
        for (let i = 0; i < data.getElementsByTagName('Section').length; i++) {
            var section = data.getElementsByTagName('Section')[i];
            if (section) {
                var sectionTitle = section.attributes['Title'].value;
                var tempSectionTemplate = sectionTemplate.split('#SectionTitle#').join(sectionTitle);
                tempSectionTemplate = tempSectionTemplate.split('#SectionCode#').join('Section' + i);

                var filledItems = '';
                for (let j = 0; j < section.getElementsByTagName('Item').length; j++) {
                    var item = section.getElementsByTagName('Item')[j];
                    if (item) {
                        var itemTitle = item.getElementsByTagName('Title')[0];
                        var itemData = item.getElementsByTagName('Data')[0];
                        if (itemTitle && itemData) {
                            if (isPublished)
                                itemData.innerHTML = itemData.innerHTML.split('/images/').join('/buddy/Demo.Tips/images/');

                            var tempItemTemplate = itemTemplate.split('#ItemCode#').join('Item' + i + '_' + j);
                            if (!itemData.innerHTML.trim())
                                itemTitle.innerHTML = '<div class="danger">' + itemTitle.innerHTML + '</div>';
                            tempItemTemplate = tempItemTemplate.split('#ItemTitle#').join(itemTitle.innerHTML);                            
                            tempItemTemplate = tempItemTemplate.split('#ItemData#').join(itemData.innerHTML);

                            filledItems += tempItemTemplate;
                        }
                    }
                }
                tempSectionTemplate = tempSectionTemplate.split('#Items#').join(filledItems);
                filledSections += tempSectionTemplate;
            }
        }
        $('#sections').append(filledSections);
    }
});