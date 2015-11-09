(function() {

    $(document).ready(function() {

        function goBack() {
            window.history.back();
        }

        function subscribe() {
            var bl = $("#backLink");
            bl.click(goBack);
        }

        subscribe();
    });

}());