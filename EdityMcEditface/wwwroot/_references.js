﻿/// <autosync enabled="true" />
/// <reference path="../bower_components/pen/src/pen.js" />
/// <reference path="../custom_components/ckeditor/config.js" />
/// <reference path="../custom_components/ckeditor/plugins/widgetbootstrap/dialogs/widgetbootstrapAlert.js" />
/// <reference path="../custom_components/ckeditor/plugins/widgetbootstrap/dialogs/widgetfoundationAccordion.js" />
/// <reference path="../custom_components/ckeditor/plugins/widgetbootstrap/plugin.js" />
/// <reference path="../custom_components/ckeditor/styles.js" />
/// <reference path="../custom_components/htmlrest/src/animate.js" />
/// <reference path="../custom_components/htmlrest/src/components.js" />
/// <reference path="../custom_components/htmlrest/src/events.js" />
/// <reference path="../custom_components/htmlrest/src/form.js" />
/// <reference path="../custom_components/htmlrest/src/lifecycle.js" />
/// <reference path="../custom_components/htmlrest/src/output.js" />
/// <reference path="../custom_components/htmlrest/src/polyfill.js" />
/// <reference path="../custom_components/htmlrest/src/rest.js" />
/// <reference path="../custom_components/htmlrest/src/storage.js" />
/// <reference path="../gulpfile.js" />
/// <reference path="edity/layouts/edit.js" />
/// <reference path="edity/layouts/editarea-ckeditor.js" />
/// <reference path="edity/layouts/editarea-pen.js" />
/// <reference path="edity/layouts/editcomponents/commit.js" />
/// <reference path="edity/layouts/editcomponents/compile.js" />
/// <reference path="edity/layouts/editcomponents/media.js" />
/// <reference path="edity/layouts/editComponents/settings.js" />
/// <reference path="edity/layouts/new.js" />
/// <reference path="lib/bootstrap/dist/js/bootstrap.js" />
/// <reference path="lib/bootstrap/dist/js/npm.js" />
/// <reference path="lib/ckeditor/ckeditor.js" />
/// <reference path="lib/ckeditor/config.js" />
/// <reference path="lib/ckeditor/lang/en.js" />
/// <reference path="lib/ckeditor/plugins/clipboard/dialogs/paste.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/af.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/ar.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/bg.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/bn.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/bs.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/ca.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/cs.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/cy.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/da.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/de.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/de-ch.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/el.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/en.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/en-au.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/en-ca.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/en-gb.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/eo.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/es.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/et.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/eu.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/fa.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/fi.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/fo.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/fr.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/fr-ca.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/gl.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/gu.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/he.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/hi.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/hr.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/hu.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/id.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/is.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/it.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/ja.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/ka.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/km.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/ko.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/ku.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/lt.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/lv.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/mk.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/mn.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/ms.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/nb.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/nl.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/no.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/pl.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/pt.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/pt-br.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/ro.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/ru.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/si.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/sk.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/sl.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/sq.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/sr.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/sr-latn.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/sv.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/th.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/tr.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/tt.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/ug.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/uk.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/vi.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/zh.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/lang/zh-cn.js" />
/// <reference path="lib/ckeditor/plugins/colorbutton/plugin.js" />
/// <reference path="lib/ckeditor/plugins/dialog/dialogDefinition.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/cs.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/da.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/de.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/de-ch.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/en.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/eo.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/eu.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/fr.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/gl.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/id.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/it.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/km.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/ko.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/ku.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/nb.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/nl.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/pl.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/pt-br.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/ru.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/sv.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/tr.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/ug.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/uk.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/zh.js" />
/// <reference path="lib/ckeditor/plugins/filetools/lang/zh-cn.js" />
/// <reference path="lib/ckeditor/plugins/filetools/plugin.js" />
/// <reference path="lib/ckeditor/plugins/image/dialogs/image.js" />
/// <reference path="lib/ckeditor/plugins/lineutils/plugin.js" />
/// <reference path="lib/ckeditor/plugins/link/dialogs/anchor.js" />
/// <reference path="lib/ckeditor/plugins/link/dialogs/link.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/cs.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/da.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/de.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/de-ch.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/en.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/eo.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/eu.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/fr.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/gl.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/id.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/it.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/km.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/ko.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/ku.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/nb.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/nl.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/pl.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/pt.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/pt-br.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/ru.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/sv.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/tr.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/ug.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/uk.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/zh.js" />
/// <reference path="lib/ckeditor/plugins/notification/lang/zh-cn.js" />
/// <reference path="lib/ckeditor/plugins/notification/plugin.js" />
/// <reference path="lib/ckeditor/plugins/notificationaggregator/plugin.js" />
/// <reference path="lib/ckeditor/plugins/panelbutton/plugin.js" />
/// <reference path="lib/ckeditor/plugins/pastefromword/filter/default.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/af.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/ar.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/bg.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/ca.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/cs.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/cy.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/da.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/de.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/de-ch.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/el.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/en.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/en-gb.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/eo.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/es.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/et.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/eu.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/fa.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/fi.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/fr.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/fr-ca.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/gl.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/he.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/hr.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/hu.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/id.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/it.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/ja.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/km.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/ko.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/ku.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/lt.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/lv.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/nb.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/nl.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/no.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/pl.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/pt.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/pt-br.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/ru.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/si.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/sk.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/sl.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/sq.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/sv.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/th.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/tr.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/tt.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/ug.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/uk.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/vi.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/zh.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/lang/zh-cn.js" />
/// <reference path="lib/ckeditor/plugins/specialchar/dialogs/specialchar.js" />
/// <reference path="lib/ckeditor/plugins/table/dialogs/table.js" />
/// <reference path="lib/ckeditor/plugins/uploadimage/plugin.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/cs.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/da.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/de.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/de-ch.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/el.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/en.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/eo.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/eu.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/fr.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/gl.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/hu.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/id.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/it.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/km.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/ko.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/ku.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/nb.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/nl.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/pl.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/pt.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/pt-br.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/ru.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/sv.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/tr.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/ug.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/uk.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/zh.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/lang/zh-cn.js" />
/// <reference path="lib/ckeditor/plugins/uploadwidget/plugin.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/af.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/ar.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/bg.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/ca.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/cs.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/cy.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/da.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/de.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/de-ch.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/el.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/en.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/en-gb.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/eo.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/es.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/eu.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/fa.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/fi.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/fr.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/gl.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/he.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/hr.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/hu.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/id.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/it.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/ja.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/km.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/ko.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/ku.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/lv.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/nb.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/nl.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/no.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/pl.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/pt.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/pt-br.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/ru.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/sk.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/sl.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/sq.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/sv.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/tr.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/tt.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/ug.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/uk.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/vi.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/zh.js" />
/// <reference path="lib/ckeditor/plugins/widget/lang/zh-cn.js" />
/// <reference path="lib/ckeditor/plugins/widget/plugin.js" />
/// <reference path="lib/ckeditor/plugins/widgetbootstrap/dialogs/widgetbootstrapAlert.js" />
/// <reference path="lib/ckeditor/plugins/widgetbootstrap/dialogs/widgetfoundationAccordion.js" />
/// <reference path="lib/ckeditor/plugins/widgetbootstrap/plugin.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/ar.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/cs.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/de.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/el.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/en.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/es.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/et.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/fi.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/fr.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/he.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/hu.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/it.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/ja.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/ko.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/nb.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/nl.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/nn.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/pl.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/pt.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/pt-br.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/ru.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/sk.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/tr.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/vi.js" />
/// <reference path="lib/ckeditor/plugins/youtube/lang/zh.js" />
/// <reference path="lib/ckeditor/plugins/youtube/plugin.js" />
/// <reference path="lib/ckeditor/styles.js" />
/// <reference path="lib/htmlrest/bundle.js" />
/// <reference path="lib/htmlrest/src/animate.js" />
/// <reference path="lib/htmlrest/src/components.js" />
/// <reference path="lib/htmlrest/src/events.js" />
/// <reference path="lib/htmlrest/src/form.js" />
/// <reference path="lib/htmlrest/src/lifecycle.js" />
/// <reference path="lib/htmlrest/src/output.js" />
/// <reference path="lib/htmlrest/src/polyfill.js" />
/// <reference path="lib/htmlrest/src/rest.js" />
/// <reference path="lib/htmlrest/src/storage.js" />
/// <reference path="lib/htmlrest/src/table.js" />
/// <reference path="lib/jquery/dist/jquery.js" />
/// <reference path="lib/pen/src/markdown.js" />
/// <reference path="lib/pen/src/pen.js" />
/// <reference path="lib/sizzle/dist/sizzle.js" />
