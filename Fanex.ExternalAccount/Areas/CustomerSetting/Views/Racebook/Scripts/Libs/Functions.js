var JAjax = {
    call: function (_options, _callback) {
        $.extend(this.options, _options);
        if (this.options.url.length > 0) {
            if ($.browser.msie && this.options.isRefresh) {
                this.options.cache = false;
            }
            JAjax.loading(true);
            $.ajax({
                type: this.options.type,
                url: this.options.url,
                contentType: this.options.contentType,
                dataType: this.options.dataType,
                data: this.options.data,
                success: function (resp) {
                    JAjax.loading(false);
                    if (typeof JAjax.options.success == 'function') {
                        JAjax.options.success(resp);
                    }
                },
                error: function (xhr, status) {
                    JAjax.loading(false);
                    if (typeof JAjax.options.error == 'function') {
                        JAjax.options.error(xhr, status);
                    }
                },
                cache: false
            });
        }
        return false;
    },
    options: {
        type: 'GET',
        url: '',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        data: null,
        success: void (0),
        error: void (0),
        timeout: 5000,
        cache: true,
        isLoading: true,
        isRefresh: false
    },
    loading: function (_status) {
        if (this.options.isLoading) {
            if (_status == true) {
                try { parent.BlockUI(); } catch (ex) { }
            } else {
                try { parent.jQuery.unblockUI(); } catch (ex) { }
            }
        }
        return false;
    }
};