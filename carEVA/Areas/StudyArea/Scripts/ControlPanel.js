//main courses menu definition
var menuItemsDefinition = [
    { Title: "Mis Cursos", TextA: "Certificados:", TextB: "En Curso:", imageName: "eva_icon_myCourses.png", page:"/mycourses" },
    { Title: "Catalogo de Cursos", TextA: "Disponibles:", TextB: "Completados:", imageName: "eva_icon_catalog.png", page: "/coursecatalog" },
    { Title: "Cursos Obligatorios", TextA: "Disponibles:", TextB: "Completados:", imageName: "eva_icon_required.png", page: "/requiredcourses" },
];



var panelMenuModel = kendo.observable({
    items: menuItemsDefinition,
    onChange: function () {
        // get a reference to the ListView widget
        var listView = $("#eva-panel").data("kendoListView");
        var selected = $.map(listView.select(), function (item) {
            return menuItemsDefinition[$(item).index()].page;
        });
        //we expect here only one selected item
        controlPanelRouter.navigate(selected[0]);
        console.log("Selected: " + selected.length + " item(s), [" + selected.join(", ") + "]");
    }
});

//kendo.bind($('#eva-panel'), panelMenuModel);

//$slideshow = $('.eva-main-slider').slick({
//    dots: true,
//    infinite: true,
//    speed: 500,
//    fade: true,
//    cssEase: 'linear',
//    lazyLoad: 'ondemand',
//    autoplay: true, 
//    autoplaySpeed: 20000,
//    pauseOnDotsHover: true,
//    touchThreshold: 20,
//});

//SPA routing

var rootLayout = new kendo.Layout("main-layout-template");
var leftMenuLayout = new kendo.Layout("left-menu-template", { model: panelMenuModel });
var welcomePageLayout = new kendo.Layout("right-welcome-page-template");
var myCoursesPageLayout = new kendo.Layout("right-myCourses-page-template");
var courseCatalogPageLayout = new kendo.Layout("right-courseCatalog-page-template");
var requiredCoursesPageLayout = new kendo.Layout("right-requiredCourses-page-template");


var controlPanelRouter = new kendo.Router({
    init: function () {
        rootLayout.render("#eva-control-panel-main-layout-anchor");
    }
});

controlPanelRouter.route("/", function () {
    rootLayout.showIn("#left-menu-anchor", leftMenuLayout);
    //temporarly default to the mycourses page
    rootLayout.showIn("#right-menu-anchor", myCoursesPageLayout);
    //rootLayout.showIn("#right-menu-anchor", welcomePageLayout);

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
});

controlPanelRouter.route("/requiredcourses", function () {
    rootLayout.showIn("#right-menu-anchor", requiredCoursesPageLayout);
});

$(function () {
    controlPanelRouter.start();
});