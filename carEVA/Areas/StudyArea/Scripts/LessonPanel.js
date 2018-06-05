//base service URL, set them according to the endpoint you wish to use
if (typeof (targetRootUrl) == 'undefined') {
    //if not defined on the calling Page, default to the cloud endpoint
    var targetRootUrl = "http://evacar.azurewebsites.net/api";
}


if (typeof (publicKey) == 'undefined' || typeof (courseID) == 'undefined') {
    alert("Ocurrio un error con tu usuario");
}

//use this values for debugging
//courseID = typeof (courseID) == 'undefined' ? 29 : courseID;
//publicKey = typeof (publicKey) == 'undefined' ? "f213e8b0-bedf-4d7b-9661-c42d0fdda4b6" : publicKey;

//global variable to store the current video URL, see if there is a better way
var videoSource = "";
var activityInstructions = "";
var minigameIframeURL = "";

//global validator
var quizValidator;

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
            data: {id: 1} //a default non existent lesson ID.
        }
    },
    pageSize: 15,
    error: function (e) {
        console.log("Error reading the files");
    }
});

var questionDataSource = new kendo.data.HierarchicalDataSource({
    transport: {
        read: function (options) {
            var temp = options;
            $.ajax({
                url: targetRootUrl + "/questiondetails",
                type: "GET",
                cache: false,
                data: { publicKey: options.data.publicKey, lessonDetailID: options.data.lessonDetailID },
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
                        response["isCorrect"] = item.detail.isCorrect;
                        response["lastGradedAnswerID"] = item.detail.lastGradedAnswerID;
                        for (var i = 0; i < response.answerOptions.length; i++) {
                            if (response.isCorrect) {
                                //disable all the radio buttons
                                response.answerOptions[i]["disabled"] = true;
                            }
                            if (response.lastGradedAnswerID === response.answerOptions[i].AnswerID) {
                                response.answerOptions[i].isCorrect = true;
                            }
                        }
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
    responses:[],
    lessonDetailID:null,
    readFileList: function (lessonID) {
        fileData.read({id:lessonID});
    },
    onSelect: function (e) {
        //detect the event on the Headers and return inmediatly
        var temp = e;
        console.log("selected item")
        var item = $(e.item);
        if (item.find(".eva-question-header").length > 0 || item.find(".eva-question-correct").length > 0) {
            //its a header element, or a correct answer group so do nothing
            return;
        }


        var panelElement = item.closest(".k-panelbar");
        var dataItem = this.questionItems.data();
        var index = item.parentsUntil(panelElement, ".k-item").map(function () {
            return $(this).index();
        }).get().reverse();

        if (!Array.isArray(index) || index.length <= 0) {
            //doublw check we clicked on a header element, so do noting
            return;

        }
        index.push(item.index());

        console.log("selected question: " + dataItem[index[0]].QuestionID
                    + "selected answer: " + dataItem[index[0]].answerOptions[index[1]].AnswerID);

        this.addOrUpdateResponse(dataItem[index[0]].QuestionID, dataItem[index[0]].answerOptions[index[1]].AnswerID);

    },
    addOrUpdateResponse: function (_questionID, _answerID) {
        for (var i = 0; i < this.responses.length; i++) {
            if (this.responses[i].get("questionID") === _questionID) {
                //since no question can have two answers
                //the question has already been answered, so update the answer value
                this.responses[i].set("answerID", _answerID);
                return;
            } 
        }
        //if we end up here, its a new question-answer pair, so add it to the array
        this.responses.push({ questionID: _questionID, answerID: _answerID });
    },
    submitResponses: function (event) {
        //responses submit handler
        event.preventDefault();
        var quizValidator = $("#eva-quiz-form").data("kendoValidator");
        if (this.get("responses").length < this.questionItems.data().length) {
            alert("hay preguntas sin responder.");
            return;
        }
        //there is a case where the answers are fully filled but not the checkboxes, so we need the validate()
        //reproduce it clicking to the left of the radiobutton
        if ((this.get("responses").length >= this.questionItems.data().length) && !quizValidator.validate()) {
            alert("hay preguntas sin responder.");
            return;
        }
        var quizResponses = { publicKey: publicKey, lessonDetailID: this.get("lessonDetailID"), responses: this.get("responses") };
        $.ajax({
            url: targetRootUrl + "/grader",
            type: "POST",
            cache: false,
            data: JSON.stringify(quizResponses),
            contentType: "application/json",
            dataType: "json",
            processData: true,
            success: function (data) {
                if (data.passed) {
                    alert('Felicitaciones, aprobaste con: ' + data.currentTotalGrade + " puntos");
                } else {
                    alert('Intenta nuevamente, obtuvisete: ' + data.currentTotalGrade + " puntos");
                }
                
            },
            error: function (response) {
                //use messages to debug the service
                alert('Error: ' + response.statusText);
            }
        });
    },
});
videoLessonModel.bind("change", function(e) {
    if (e.field == "questionItems") {
        var newItems = questionDataSource.data();
        for (var i = 0; i < newItems.length; i++) {
            if (newItems[i].isCorrect) {
                videoLessonModel.addOrUpdateResponse(newItems[i].QuestionID, newItems[i].lastGradedAnswerID);
            }
        }
    }
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

var miniGameModel = kendo.observable({
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
var miniGameLayout = new kendo.Layout('right-minigame-iframe-template', { model: miniGameModel })

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
                                        miniGameURL: data[$(item).index()].info.interactiveActivityURL,
                                        lessonIdx: $(item).index(),
                                    };
                                });

                        //we expect it to always be an array of just one element
                        //set the global variables
                        videoSource = selected[0].videoURL;
                        activityInstructions = selected[0].activityInstructions;
                        minigameIframeURL = selected[0].miniGameURL;

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
    //check the type of the lesson to render
    var lessonType = lessonsData.data()[rootIdx].lessons[lessonIdx].info.lessonType;

    switch (lessonType) {
        case "VideoLesson":
            //implement the logic to show the corresponding content on the panel
            //query files and questions with the correct information
            fileData.read({ id: lessonID });
            questionDataSource.read({ publicKey: publicKey, lessonDetailID: lessonDetailID });
            videoLessonModel.set("lessonDetailID", lessonDetailID);

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
            $("#eva-quiz-form").kendoValidator({
                rules: {
                    radio: function (input) {
                        if (input.filter("[type=radio]") && input.attr("required")) {
                            return $("#eva-quiz-form").find("[name=" + input.attr("name") + "]").is(":checked");
                        }
                        return true;
                    }
                },
                messages: {
                    radio: "Debes responder la pregunta",
                },
            });
            break;
        case "ActivityUpload":
            rootLayout.showIn("#right-panel-anchor", righActivityUploadLayout);
            //html replace
            //TODO: see if there is a way of doing this with model
            $("#eva-activity-upload").empty().html(activityInstructions);
            //TODO: call the detail to upload de file
            break;
        case "Infograph":
            rootLayout.showIn("#right-panel-anchor", miniGameLayout);
            //TODO take special care of this
            //a little hack to check if we are in debug
            if (debugNET) {
                minigameIframeURL = minigameIframeURL.replace("evacar.azurewebsites.net", "localhost:63052");
            }

            //change the source of the iframe to display the minigame
            $("#miniGameIframe").attr('src', minigameIframeURL);
            break;
        case "Crossword":
            rootLayout.showIn("#right-panel-anchor", miniGameLayout);
            //TODO take special care of this
            //a little hack to check if we are in debug
            if (debugNET) {
                minigameIframeURL = minigameIframeURL.replace("evacar.azurewebsites.net", "localhost:63052");
            }

            //change the source of the iframe to display the minigame
            $("#miniGameIframe").attr('src', minigameIframeURL);
            break;
        default:
    }
});


$(function () {
    lessonPanelRouter.start();
});