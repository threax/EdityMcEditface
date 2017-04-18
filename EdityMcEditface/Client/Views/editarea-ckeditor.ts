///<amd-module name="edity.core.editarea-ckeditor"/>

"use strict";

import * as storage from "hr.storage";
import * as controller from "hr.controller";
import * as navmenu from "edity.editorcore.navmenu";
import * as pageService from "edity.editorcore.PageService";
import * as PageStart from "edity.editorcore.EditorPageStart";

PageStart.init().then(pageConfig => {
    var editor = undefined;

    var CKEDITOR = (<any>window).CKEDITOR;

    CKEDITOR.editorConfig = function (config) {
        config.toolbarGroups = [
            { name: 'clipboard', groups: ['clipboard', 'undo'] },
            { name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
            { name: 'links', groups: ['links'] },
            { name: 'insert', groups: ['insert'] },
            { name: 'forms', groups: ['forms'] },
            { name: 'tools', groups: ['tools'] },
            { name: 'document', groups: ['mode', 'document', 'doctools'] },
            { name: 'others', groups: ['others'] },
            '/',
            { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
            { name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
            { name: 'styles', groups: ['styles'] },
            { name: 'colors', groups: ['colors'] },
            { name: 'about', groups: ['about'] }
        ];

        config.removeButtons = 'Underline,Subscript,Superscript,Scayt,Maximize,Source,About,Add alert box';

        config.allowedContent = true;
        config.extraPlugins = 'colorbutton,youtube,edityimageupload,widgetbootstrap';
        config.imageUploadUrl = pageConfig.BaseUrl + '/edity/Page/AddAsset?page=' + window.location.pathname;
    };

    CKEDITOR.on('instanceReady', function (ev) {
        editor = ev.editor;
        var keys = ["body", "head", "html", "title", "base", "command", "link", "meta", "noscript", "script", "style", "audio", "dd", "dt",
            "figcaption", "video", "address", "article", "aside", "blockquote", "details", "div", "dl", "fieldset", "figure",
            "footer", "form", "h1", "h2", "h3", "h4", "h5", "h6", "header", "hgroup", "hr", "main", "menu", "nav", "p", "pre",
            "section", "table", "ul", "center", "dir", "noframes", "caption", "col", "colgroup", "tbody", "td", "tfoot", "th", "thead", "tr", "br", "ol"];

        for (var i = 0; i < keys.length; ++i) {
            ev.editor.dataProcessor.writer.setRules(keys[i], {
                indent: true,
                breakBeforeOpen: true,
                breakAfterOpen: false,
                breakBeforeClose: false,
                breakAfterClose: false
            });
        }

        editor.on('change', function () {
            pageService.sourceUpdated();
        });
    });

    pageService.setSourceAccessor({
        getHtml: function () {
            return editor.getData();
        },
        setHtml: function (value) {
            editor.setData(value);
        }
    }); 
});