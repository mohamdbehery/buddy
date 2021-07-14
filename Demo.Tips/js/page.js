$(document).ready(function () {
    var url = window.location.href.indexOf('github') > 0 ? "/buddy/Demo.Tips" : "" + "/review-source.xml";

    $.ajax({
        type: "GET",
        url: url,
        dataType: "xml",
        success: function (xml) {
            if (xml.children[0]) {
                var sectionTemplate = $('#section-template').html();
                var itemTemplate = $('#item-template').html();

                var filledSections = '';
                for (let i = 0; i < xml.children[0].getElementsByTagName('Section').length; i++) {
                    var section = xml.children[0].getElementsByTagName('Section')[i];
                    if (section) {
                        var sectionTitle = section.attributes['Title'].value;
                        var tempSectionTemplate = sectionTemplate.split('#SectionTitle#').join(sectionTitle);
                        tempSectionTemplate = tempSectionTemplate.split('#SectionCode#').join('Section' + i);

                        var filledItems = '';
                        for (let j = 0; j < section.getElementsByTagName('Item').length; j++) {
                            var item = section.getElementsByTagName('Item')[j];
                            if (item) {
                                var title = item.getElementsByTagName('Title')[0];
                                var data = item.getElementsByTagName('Data')[0];
                                if (title && data) {
                                    var dataContent = data.innerHTML.split('/images/').join('/buddy/Demo.Tips/images/');

                                    var tempItemTemplate = itemTemplate.split('#ItemCode#').join('Item' + i + '_' + j);
                                    tempItemTemplate = tempItemTemplate.split('#ItemTitle#').join(title.innerHTML);
                                    tempItemTemplate = tempItemTemplate.split('#ItemData#').join(dataContent);

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
        }
    });
});