//base service URL, set them according to the endpoint you wish to use
//var targetRootUrl = "http://evacar.azurewebsites.net/api"; //production endpoint 
var targetRootUrl = "http://localhost:63052/api"; //local host eviroment

if (typeof (publicKey) == 'undefined' || typeof (courseID) == 'undefined') {
    alert("Ocurrio un error con tu usuario");
}

//use this values for debugging
//courseID = typeof (courseID) == 'undefined' ? 29 : courseID;
//publicKey = typeof (publicKey) == 'undefined' ? "f213e8b0-bedf-4d7b-9661-c42d0fdda4b6" : publicKey;

//global variable to store the current video URL, see if there is a better way
var videoSource = "";
var activityInstructions = "";

var lessonsData = new kendo.data.DataSource({
    transport: {
        read: {
            url: targetRootUrl + "/lessonDetail?courseID="+ courseID.toString() +"&publicKey=" + publicKey,
            dataType: "json"
        }
    },
    pageSize: 15,
    error: function (e) {
        console.log("consultando");
    }
});

var fileData = new kendo.data.DataSource({
    transport: {
        read: {
            url: targetRootUrl + "/evafiles",
            dataType: "json",
            data: {id: 93}
        }
    },
    pageSize: 15,
    error: function (e) {
        console.log("error reading fileData on: ");
    }
});

var questionDataSource = new kendo.data.HierarchicalDataSource({
    transport: {
        read: function (options) {
            $.ajax({
                url: targetRootUrl + "/questiondetails",
                type: "GET",
                cache: false,
                data: { publicKey: publicKey, lessonDetailID: 1101 },
                contentType: "application/json",
                dataType: "json",
                processData: true,
                traditional: true,
                success: function (data) {
                    //format the data, get only the question list from the service
                    //parse the response:
                    newData = $.map(data.quizDetail, function (item, index) {
                        var response = {};
                        for (var property in item.question) {
                            if (item.question.hasOwnProperty(property)) {
                                response[property] = item.question[property]
                            }
                        }
                        //create a new property
                        response["detail"] = item.detail;
                        return response;
                    });
                    options.success(newData);
                },
                error: function (response) {
                    //use messages to debug the service
                    console.log("error reading question data on: ");
                    options.error(response);
                }
            });
        },
    },
    schema: {
        model: {
            children: "answerOptions"
        }
    }
});

var videoLessonModel = kendo.observable({
    fileItems: fileData,
    questionItems: questionDataSource,
    readFileList: function (lessonID) {
        fileData.read({id:lessonID});
    },
    onSelect: function () {
        console.log("selected item")
    },
});


var activityUploadModel = kendo.observable({
    fileItems: fileData,
    questionItems: questionDataSource,
    readFileList: function (lessonID) {
        fileData.read({ id: lessonID });
    },
    onSelect: function () {
        console.log("selected item")
    },
});
//var lessonsModel = kendo.observable({
//    items: lessonsData,
//    onBound: function (e) {
//        //bind the children
//        $.each(this.get("items"), function (idx, item) {
//            $("#child-lesson-listview-" + item.chapter.index).kendoListView({
//                template: $("#lesson-item-template").html(),
//                dataSource: item.lessons
//            })
//        });
//    },
//    onClick: function (e) {
//        var temp = e.data;
//        console.log("click on the courseID: " + temp.courseID.toString());
//        return temp;
//    },
//});

var rootLayout = new kendo.Layout("main-layout-template");
var leftMenuLayout = new kendo.Layout("left-lessons-template");
var welcomePageLayout = new kendo.Layout("right-course-welcome-template");
var righVideoPanelLayout = new kendo.Layout("right-video-template", { model: videoLessonModel });
var righActivityUploadLayout = new kendo.Layout("right-activity-upload-template", { model: activityUploadModel });

var lessonPanelRouter = new kendo.Router({
    init: function () {
        rootLayout.render("#eva-lesson-panel-main-layout-anchor");
    },
    routeMissing: function (e) {
        console.log(e.url)
    }
});

lessonPanelRouter.route("/", function () {
    rootLayout.showIn("#left-panel-anchor", leftMenuLayout);
    rootLayout.showIn("#right-panel-anchor", welcomePageLayout);

    //everything that depends on a elements that its inside a template
    //must be instantiated here as
    //the elements only exists on the DOM the moment its renderer
    //after the showIn

    $("#eva-chapters-panel").kendoListView({
        template: kendo.template($("#chapter-item-template").html()),
        dataSource: lessonsData,
        dataBound: function (e) {
            // Bind children
            $.each(this.dataItems(), function (idx, item) {
                $("#child-lesson-listview-" + item.chapter.index).kendoListView({
                    template: kendo.template($("#lesson-item-template").html()),
                    selectable: "single",
                    dataSource: item.lessons,
                    change: function (e) {
                        //get the click information, take special care to the fact that there are
                        //multiple lessons lists on the UI.
                        //see https://stackoverflow.com/questions/19699134/nested-datasources-in-kendo-ui
                        $.map($(".eva-lesson-list-container"), function (item) {
                            if (item != e.sender.element.context && $(item).attr('data-evaselected') == "true") {
                                //use the cleared state so we can identify when the event is triggered by the clearSection()
                                $(item).attr('data-evaselected', "cleared");
                                $(item).data("kendoListView").clearSelection();
                            }
                        });
                        //$(this.element.context).data('evaselected', true);
                        if ($(this.element.context).attr('data-evaselected') == "cleared") {
                            $(this.element.context).attr('data-evaselected', "false");
                            return;
                        }
                        else {
                            $(this.element.context).attr('data-evaselected', "true");
                        }
                        //$(this).data('evaselected', true);
                        var data = item.lessons,
                                selected = $.map(this.select(), function (item) {
                                    return {
                                        LessonID: data[$(item).index()].info.LessonID,
                                        lessonDetailID: data[$(item).index()].userDetail.evaLessonDetailID,
                                        videoURL: data[$(item).index()].info.videoURL,
                                        activityInstructions: data[$(item).index()].info.activityInstructions,
                                        lessonIdx: $(item).index(),
                                    };
                                });

                        //we expect it to always be an array of just one element
                        //set the global variables
                        videoSource = selected[0].videoURL;
                        activityInstructions = selected[0].activityInstructions;

                        console.log("lessonID: " + selected[0].LessonID +
                            " lessonDetailID: " + selected[0].lessonDetailID +
                            " videoURL " + selected[0].videoURL +
                            " rootObjetcIdx: " + idx );
                        lessonPanelRouter.navigate("/content/" + selected[0].LessonID + "/"
                            + selected[0].lessonDetailID + "/" + idx + "/" + selected[0].lessonIdx);
                    }
                })
            })
        },

    });
});

//rootIdx represent the index of the first level of objets, so we can fully navigate the lesson detail structure
lessonPanelRouter.route("/content/:lessonID/:lessonDetailID/:rootIdx/:lessonIdx"
    , function (lessonID, lessonDetailID, rootIdx, lessonIdx) {
    //if there are no parameters on the route, navigate to the default page
    if (lessonID === "undefined" || lessonDetailID === "undefined") {
        lessonPanelRouter.navigate("/");
        return;
    }

    if ($("#eva-video")[0] != null) {
        //use this to force video unload, as it keeps playing after beign hidden.
        //TODO: check here for performance
        $("#eva-video")[0].pause();
        $("#eva-video source").attr('src', "");
        $("#eva-video")[0].load();
    }

    var temp = lessonsData.data();
    //check the type of the lesson to render
    var lessonType = lessonsData.data()[rootIdx].lessons[lessonIdx].info.lessonType;

    switch (lessonType) {
        case "VideoLesson":
            //implement the logic to show the corresponding content on the panel
            rootLayout.showIn("#right-panel-anchor", righVideoPanelLayout);
            $("#eva-video source").attr('src', videoSource);
            $("#eva-video")[0].load();

            $("#tabstrip").kendoTabStrip({
                animation: {
                    open: {
                        effects: "fadeIn"
                    }
                }
            });
            break;
        case "ActivityUpload":
            rootLayout.showIn("#right-panel-anchor", righActivityUploadLayout);
            //html replace
            //TODO: see if there is a way of doing this with model
            $("#eva-activity-upload").empty().html(activityInstructions);
            //TODO: call the detail to upload de file
            break;
        default:
    }
});


$(function () {
    lessonPanelRouter.start();
});