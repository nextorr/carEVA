//base service URL, set them according to the endpoint you wish to use
if (typeof (targetRootUrl) == 'undefined') {
    //if not defined on the calling Page, default to the cloud endpoint
    var targetRootUrl = "http://evacar.azurewebsites.net/api";
}


if (typeof (publicKey) == 'undefined') {
    alert("Ocurrio un error con tu usuario");
}

//use this default publicKey For testing
//publicKey = typeof (publicKey) == 'undefined' ? "f213e8b0-bedf-4d7b-9661-c42d0fdda4b6" : publicKey;

var courseCatalogData = new kendo.data.DataSource({
    transport: {
        read: {
            url: targetRootUrl + "/courseCatalog?publicKey=" + publicKey,
            dataType: "json"
        }
    },
    pageSize: 15,
    error: function (e) {
        console.log("error on course catalog");
    }
});

var myCoursesData = new kendo.data.DataSource({
    transport: {
        read: {
            url: targetRootUrl + "/courseEnrollments?publicKey=" + publicKey,
            dataType: "json"
        }
    },
    pageSize: 15,
    error: function (e) {
        console.log("error on my courses");
    }
});

var scoreData = new kendo.data.DataSource({
    transport: {
        read: {
            url: targetRootUrl + "/score?publicKey=" + publicKey,
            dataType: "json"
        }
    },
    schema: {
        parse: function (response) {
            var menuItemsDefinition = [
                {
                    Title: "Mis Cursos",
                    TextA: "Certificados:",
                    TextB: "En Curso:",
                    imageName: "eva_icon_myCourses.png",
                    page: "/mycourses",
                    scoreA: response.completedCatalogCourses,
                    scoreB: response.totalActiveEnrollments,
                },
                {
                    Title: "Catalogo de Cursos",
                    TextA: "Disponibles:",
                    TextB: "Completados:",
                    imageName: "eva_icon_catalog.png",
                    page: "/coursecatalog",
                    scoreA:response.totalCatalogCourses,
                    scoreB:response.completedCatalogCourses,
                },
                {
                    Title: "Cursos Obligatorios",
                    TextA: "Disponibles:",
                    TextB: "Completados:",
                    imageName: "eva_icon_required.png",
                    page: "/requiredcourses",
                    scoreA:response.totalRequiredCourses,
                    scoreB: response.completedRequiredCourses,
                },
            ];
            return menuItemsDefinition;
        }
    },
    error: function (e) {
        console.log("error on score");
}
});


var panelMenuModel = kendo.observable({
    items: scoreData,
    onChange: function () {
        // get a reference to the ListView widget
        var listView = $("#eva-panel").data("kendoListView");
        var selected = $.map(listView.select(), function (item) {
            return scoreData.data()[$(item).index()].page;
        });
        //we expect here only one selected item
        controlPanelRouter.navigate(selected[0]);
        console.log("Selected: " + selected.length + " item(s), [" + selected.join(", ") + "]");
    }
});

var courseCatalogModel = kendo.observable({
    items: courseCatalogData,
    onClick: function (e) {
        var temp = e.data;
        console.log("click on the courseID: " + temp.courseID.toString());
        
        return temp;
    },
});

var myCoursesModel = kendo.observable({
    items: myCoursesData,
    onClick: function (e) {
        var temp = e.data;
        console.log("click on the evaCourseEnrollmentID: " + temp.enrollment.evaCourseEnrollmentID.toString());
        location.href = '/StudyArea/dashboard/LessonArea/' + temp.enrollment.CourseID.toString();
        return temp;
    },
});

//SPA routing

var rootLayout = new kendo.Layout("main-layout-template");
var leftMenuLayout = new kendo.Layout("left-menu-template", { model: panelMenuModel });
var welcomePageLayout = new kendo.Layout("right-welcome-page-template");
var myCoursesPageLayout = new kendo.Layout("right-myCourses-page-template", { model: myCoursesModel });
var courseCatalogPageLayout = new kendo.Layout("right-courseCatalog-page-template", {model: courseCatalogModel});


var controlPanelRouter = new kendo.Router({
    init: function () {
        rootLayout.render("#eva-control-panel-main-layout-anchor");
    }
});

controlPanelRouter.route("/", function () {
    rootLayout.showIn("#left-menu-anchor", leftMenuLayout);
    rootLayout.showIn("#right-menu-anchor", welcomePageLayout);

    //everything that depends on a elements that its inside a template
    //must be instantiated here as
    //the elements only exists on the DOM the moment its renderer
    //after the showIn
    $slideshow = $('.eva-main-slider').slick({
        dots: true,
        infinite: true,
        speed: 500,
        fade: true,
        cssEase: 'linear',
        lazyLoad: 'ondemand',
        autoplay: true,
        autoplaySpeed: 20000,
        pauseOnDotsHover: true,
        touchThreshold: 20,
    });
});

controlPanelRouter.route("/mycourses", function () {
    rootLayout.showIn("#right-menu-anchor", myCoursesPageLayout);
});

controlPanelRouter.route("/coursecatalog", function () {
    rootLayout.showIn("#right-menu-anchor", courseCatalogPageLayout);
    courseCatalogData.filter({ field: "required", operator: "eq", value: false });
});

controlPanelRouter.route("/requiredcourses", function () {
    rootLayout.showIn("#right-menu-anchor", courseCatalogPageLayout);
    courseCatalogData.filter({ field: "required", operator: "eq", value: true });
});

$(function () {
    controlPanelRouter.start();
});