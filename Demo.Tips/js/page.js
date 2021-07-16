$(document).ready(() => {

    $.ajax({
        type: "GET",
        url: url,
        dataType: "xml",
        success: (xml) => {
            if (xml.children[0]) {
                BindSections(xml.children[0]);
            }
        },
        error: (data) => {
            console.log(data);
        }
    });

    $('#ShowEmptyAll').on('click', () => {
        if ($('#ShowEmptyAll').hasClass('all')) {
            $('.all-items-filled').hide();
            $('.item-filled').hide();
            $('#ShowEmptyAll').text('[Show All]');
            $('#ShowEmptyAll').removeClass('all');
        }
        else {
            $('.all-items-filled').show();
            $('.item-filled').show();
            $('#ShowEmptyAll').text('[Show Empty]');
            $('#ShowEmptyAll').addClass('all');
        }
    });
});

var isPublished = window.location.href.indexOf('github') > 0 ? true : false;
var url = (isPublished ? "/buddy/Demo.Tips" : "") + "/review-source.xml";

var BindSections = (data) => {
    var sectionTemplate = $('#section-template').html();

    var filledSections = '';
    for (let i = 0; i < data.getElementsByTagName('Section').length; i++) {
        var section = data.getElementsByTagName('Section')[i];
        if (section) {
            var sectionTitle = section.attributes['Title'].value;
            var tempSectionTemplate = sectionTemplate.split('#SectionTitle#').join(sectionTitle);
            tempSectionTemplate = tempSectionTemplate.split('#SectionCode#').join('Section' + i);
            var sectionData = BindSectionData(section, i);
            tempSectionTemplate = tempSectionTemplate.split('#Items#').join(sectionData.FilledItems);
            tempSectionTemplate = tempSectionTemplate.split('#SectionStatus#').join(sectionData.AreAllItemsFilled ? 'all-items-filled' : '');
            filledSections += tempSectionTemplate;
        }
    }
    $('#sections').append(filledSections);
};

var BindSectionData = (section, sectionIndex) => {
    var sectionData = {
        FilledItems: '',
        AreAllItemsFilled: true
    };

    var itemTemplate = $('#item-template').html();
    for (let j = 0; j < section.getElementsByTagName('Item').length; j++) {
        var item = section.getElementsByTagName('Item')[j];
        if (item) {
            var itemTitle = item.getElementsByTagName('Title')[0];
            var itemData = item.getElementsByTagName('Data')[0];

            if (itemTitle && itemData) {
                var tempItemTemplate = itemTemplate;

                // if itemData has content OR its section is for questions to ask not to answer
                if (itemData.innerHTML.trim() || section.attributes['Title'].value.includes('Ask')) {
                    tempItemTemplate = tempItemTemplate.split('#ItemStatus#').join('item-filled');
                    if (isPublished)
                        itemData.innerHTML = itemData.innerHTML.split('/images/').join('/buddy/Demo.Tips/images/');
                }
                else {
                    tempItemTemplate = tempItemTemplate.split('#ItemStatus#').join('item-empty');
                    sectionData.AreAllItemsFilled = false;
                }
                tempItemTemplate = tempItemTemplate.split('#ItemCode#').join('Item' + sectionIndex + '_' + j);
                tempItemTemplate = tempItemTemplate.split('#ItemTitle#').join(itemTitle.innerHTML);
                tempItemTemplate = tempItemTemplate.split('#ItemData#').join(itemData.innerHTML);
                sectionData.FilledItems += tempItemTemplate;
            }
        }
    }
    return sectionData;
};