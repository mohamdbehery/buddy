$(document).ready(function () {
    $('#test').html('Test');
    $.ajax({
        type: "GET",
        url: "/review-source.xml",
        dataType: "xml",
        success: function (xml) {
            debugger;
            if (xml.children[0]) {
                var sectionTemplate = $('#section-template').html();
                var itemTemplate = $('#item-template').html();

                var filledSections = '';
                for (let i = 0; i < xml.children[0].getElementsByTagName('Section').length; i++) {
                    var section = xml.children[0].getElementsByTagName('Section')[i];
                    var sectionTitle = section.attributes['Title'].value;
                    var tempSectionTemplate = sectionTemplate.split('#SectionTitle#').join(sectionTitle);
                    tempSectionTemplate = tempSectionTemplate.split('#SectionCode#').join('Section' + i);
                    
                    var filledItems = '';
                    for (let j = 0; j < section.getElementsByTagName('Item').length; j++) {
                        var item = section.getElementsByTagName('Item')[j];
                        var title = item.getElementsByTagName('Title')[0];
                        var data = item.getElementsByTagName('Data')[0];
                        var tempItemTemplate = itemTemplate.split('#ItemTitle#').join(title.innerHTML);
                        tempItemTemplate = tempItemTemplate.split('#ItemData#').join(data.innerHTML);
                        tempItemTemplate = tempItemTemplate.split('#ItemCode#').join('Item' + i + '_' + j);
                        filledItems += tempItemTemplate;
                    }
                    tempSectionTemplate = tempSectionTemplate.split('#Items#').join(filledItems);
                    filledSections += tempSectionTemplate;
                }
                debugger;
                $('#sections').append(filledSections);
            }
        }
    });
});