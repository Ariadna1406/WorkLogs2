document.addEventListener('DOMContentLoaded', function () {
    const validValues = [0.10, 0.20, 0.25, 0.30, 0.40, 0.50, 1.00];  // Массив допустимых значений для интенсивности выполнения работ

    var holidays  = [
        "01-05-2025",
        "02-05-2025",
        "08-05-2025",
        "09-05-2025"
    ]

    var format_date = gantt.date.str_to_date("%d-%m-%Y");

    // Исключение часов в праздничные дни
    for (var i = 0; i < holidays.length; i++) {
        var converted_date = format_date(holidays[i])
        gantt.setWorkTime({date:converted_date, hours:false})
    }

    // Проверка праздничного дня
    function isHoliday(date) {
        var formattedDate = gantt.date.date_to_str("%d-%m-%Y")(date);
        return holidays.indexOf(formattedDate) !== -1;
    }
 
    // Выделение цветом выходных и праздничных дней недели
    gantt.templates.scale_cell_class = function(date){
        if(!gantt.isWorkTime(date)) {
            return isHoliday(date) ? "holiday" : "weekend";
        };
    };

    // Выделение выходных и праздничных дней на диаграмме
    gantt.templates.timeline_cell_class = function (item, date) {
        if (!gantt.isWorkTime(date)) {
            return isHoliday(date) ? "holiday" : "weekend";
        }
    }

    // Конфигурация графика gantt
    gantt.config.row_height = 40;   // Высота строк
    gantt.config.date_format = "%d-%m-%Y";   // Формат даты
    gantt.config.show_grid = true; // Отображение сетки
    gantt.config.show_add_button = true; // Отображение кнопки добавления (плюсик)
    gantt.config.work_time = true; // Учет только рабочих дней при расчете длительности
    // gantt.config.resize_rows = true; // Изменение высоты строк в таблице
    gantt.config.sort = true; // Сортировка столбцов
    // gantt.config.order_branch = true; // Перетаскивание задач
    gantt.config.bar_height = 25; // Высота полосок на диаграмме
    gantt.i18n.setLocale("ru"); // Русский язык

    // Дни недели и число
    gantt.config.scales = [
        { unit: "day", step: 1, format: "%D" },
        { unit: "day", step: 1, format: "%d.%m" },
    ]
    gantt.config.scale_height = 50; // Высота шапочки
    gantt.config.min_column_width = 70; // Ширина ячеек сетки диаграммы

    // Проекты
    const projects = [
        { key: "3045", label: "3045"},
        { key: "1092-ИНКО(2946)", label: "1092-ИНКО(2946)"},
        { key: "И029 (2946)", label: "И029 (2946)"}
    ]

    // Комплекты с привязкой к проектам
    const sets = [
        { key: "2595-3045-ЭкП-012(1200)-ТХ1_и2", label: "2595-3045-ЭкП-012(1200)-ТХ1_и2", project: "3045"},
        { key: "2595-3045-ЭкП-012(1200)-ТХ1.1", label: "2595-3045-ЭкП-012(1200)-ТХ1.1", project: "3045"},
        // { key: "1092-ЭкП-И029-ИОС.ТР1.1", label: "1092-ЭкП-И029-ИОС.ТР1.1", project: "1092-ИНКО(2946)"},
        { key: "1092-ЭкП-008(0807)-И029-КЖ1.ЛС", label: "1092-ЭкП-008(0807)-И029-КЖ1.ЛС", project: "И029 (2946)"},
        { key: "1092-ЭкП-001(0100)-И029-ТХ13-АН-010-24-ЛС", label: "1092-ЭкП-001(0100)-И029-ТХ13-АН-010-24-ЛС", project: "И029 (2946)"},
    ]

    // Ресурсы (сотрудники)
    gantt.config.resources = [
        { key: 1, label: "Мосина Екатерина Игоревна"},
        { key: 2, label: "Султанова Диляра Ильдаровна"},
        { key: 3, label: "Никонов Дмитрий Андреевич"},
    ];

    // Колонки в таблице
    gantt.config.columns = [
        { name: "project", label: "Проект", min_width: 150, align: "left" },
        { name: "text", label: "Комплект", min_width: 240, align: "left" },
        { name: "work", label: "Выполняемая работа", min_width: 250, align: "left" },
        { name: "resources", label: "Сотрудник", align: "center", min_width: 230, template: function (task) {
            if (task.resources) {
                var resourceNames = task.resources.map(function (id) {
                    var res = gantt.config.resources.find(function (r) {
                        return r.key === id;
                    });
                    return res ? res.label : id;
                });

                return resourceNames.join(', ');
            }
            return "";
        }},
        { name: "start_date", label: "Дата начала", align: "center", min_width: 100},
        { name: "duration", label: "Длительность", align: "center", min_width: 100, template: function(task) {
            return task.original_duration || task.duration;
        } },
        { name: "intensity", label: "Интенсивность", min_width: 110, align: "center", template: function(task) {
            return (task.intensity || 1.00).toFixed(2);
        }},
        { name: "planned-progress", label: "Планир. % выполнения", min_width: 150, align: "center", template: function(task) {
            return (task.planned_progress || 0);
        }},
        { name: "total_hours", label: "Итого, ч", align: "center", min_width: 70, template: function(task) {
            let duration = task.duration || 0;
            let intensity = task.intensity || 1.00;
            return (duration * 8 * intensity);
        }},
        { name: "add", min_width: 44, align: "center"} // Колонка для кнопки добавления
    ];

    // Исходные данные для таблицы
    const data = [
        { id: 1, project: "3045", text: "2595-3045-ЭкП-012(1200)-ТХ1_и2", work: "Расчетная часть", resources: [1], start_date: "05-03-2025", duration: 8, intensity: 0.50, planned_progress: 80 },
        { id: 2,  project: "3045", text: "2595-3045-ЭкП-012(1200)-ТХ1_и2", work: "Оформление текстовой части", resources: [2], start_date: "07-05-2025", duration: 10, intensity: 1.00, planned_progress: 100 },
        { id: 3,  project: "3045", text: "2595-3045-ЭкП-012(1200)-ТХ1_и2", work: "Оформление графической части", resources: [3], start_date: "12-05-2025", duration: 8, intensity: 1.00, planned_progress: 100 },
        { id: 4, project: "3045", text: "2595-3045-ЭкП-012(1200)-ТХ1.1", work: "Расчетная часть", resources: [2], start_date: "06-05-2025", duration: 6, intensity: 0.50, planned_progress: 80 },
        { id: 5, project: "3045", text: "2595-3045-ЭкП-012(1200)-ТХ1.1", work: "Оформление текстовой и графической части", resources: [1], start_date: "13-05-2025", duration: 7, intensity: 1.00, planned_progress: 100 },
        { id: 6, project: "1092-ИНКО(2946)", text: "1092-ЭкП-И029-ИОС.ТР1.1", work: "Расчетная часть", resources: [3], start_date: "05-05-2025", duration: 9, intensity: 0.50, planned_progress: 80 },
        { id: 7, project: "1092-ИНКО(2946)", text: "1092-ЭкП-И029-ИОС.ТР1.1", work: "Оформление текстовой части", resources: [2], start_date: "07-05-2025", duration: 7, intensity: 1.00, planned_progress: 100 },
        { id: 8, project: "1092-ИНКО(2946)", text: "1092-ЭкП-И029-ИОС.ТР1.1", work: "Оформление графической части", resources: [3], start_date: "12-05-2025", duration: 5, intensity: 1.00, planned_progress: 100 }
    ];

    // Блок для настройки окна добавления задачи
    gantt.form_blocks["my_description"] = { 
        render: function (sns) { 
            return `<div class='dhx_cal_ltext' style='height:380px; padding-top: 10px; margin-bottom: 20px'>
            <label class='new_work'>Проект</label><br>
            <select class='editor_project' type='text' style="width: 100%; margin-bottom: 15px;"></select><br>

            <label class='new_work'>Комплект</label><br>
            <select class='editor_text' type='text' style="width: 100%; margin-bottom: 15px;"></select><br>
            
            <label class='new_work'>Выполняемая работа</label><br>
            <input class='editor_work' type='text' style="width: 100%; margin-bottom: 15px;"><br>

            <label class='new_work'>Сотрудник</label><br>
            <select class='editor_resources' style="width: 100%; margin-bottom: 15px;"></select>

            <label class='new_work'>Интенсивность</label><br>
            <div class="editor_intensity_container">
                <button class="editor_intensity_button" data-action="decrease">-</button>
                <input class='editor_intensity' type='text' readonly/>
                <button class="editor_intensity_button" data-action="increase">+</button>
            </div>
            <br>
            
            <label class='new_work'>Планир. % выполнения</label><br>
            <input class='editor_planned_progress' type='number' min='0' max='100' step='5' style="width: 70px; margin-bottom: 10px;"><br>

            </div>`; 
        }, 
        set_value: function (node, value, task) {
            let projectSelect = node.querySelector(".editor_project");
            let setSelect = node.querySelector(".editor_text");

            // Заполнеине списка проектов
            projectSelect.innerHTML = projects.map(p => `<option value'${p.key}'>${p.label}</option>`).join("");
            projectSelect.value = task.project || "";

            // Функция обновления списка комплектов в зависимости от выбранного проекта
            function updateSetList(selectedProject) {
                setSelect.innerHTML = "";
                if (!selectedProject) { 
                    setSelect.innerHTML = "<option value='' disabled selected>Выберите проект</option>";
                } else {
                    const setsForProject = sets.filter(s => s.project === selectedProject);
                    if (setsForProject.length === 0) {
                        setSelect.innerHTML = "<option value='' disabled selected>Нет комплектов</option>";
                    } else {
                        setSelect.innerHTML = "<option value='' disabled selected></option>" + setsForProject.map(s => `<option value='${s.key}'>${s.label}</option>`).join("");
                    }
                }
            }

            // Обновление списка комплектов при изменении проекта
            projectSelect.addEventListener("change", function () {
                updateSetList(projectSelect.value);
            });

            // Заполнение списка комлпектов при открытии окна
            updateSetList(task.project);
            setSelect.value = task.text || "";

            node.querySelector(".editor_work").value = task.work || "";

            var select = node.querySelector(".editor_resources"); 
            select.innerHTML = "<option value=''></option>" +
                gantt.config.resources.map(function (resource) { 
                    return "<option value='" + resource.key + "'>" + resource.label + "</option>"; 
                }).join(""); 
            select.value = task.resources && task.resources[0] || ""; 

            let intensityInput = node.querySelector(".editor_intensity");
            intensityInput.value = (task.intensity || 1.00).toFixed(2);

            node.querySelector(".editor_planned_progress").value = task.planned_progress || 100;

            let intensityInputElemenet = node.querySelector(".editor_intensity");
            let decreaseButton = node.querySelector(".editor_intensity_button[data-action='decrease']");
            let increaseButton = node.querySelector(".editor_intensity_button[data-action='increase']");

            decreaseButton.addEventListener('click', function () {
                let currentIndex = validValues.indexOf(parseFloat(intensityInputElemenet.value));
                if (currentIndex > 0) {
                    intensityInputElemenet.value = validValues[currentIndex - 1].toFixed(2);
                } 
            })

            increaseButton.addEventListener('click', function () {
                let currentIndex = validValues.indexOf(parseFloat(intensityInputElemenet.value));
                if (currentIndex < validValues.length - 1) {
                    intensityInputElemenet.value = validValues[currentIndex + 1].toFixed(2);
                }
            })
        }, 
        get_value: function (node, task) { 
            task.project = node.querySelector(".editor_project").value;
            task.text = node.querySelector(".editor_text").value;
            task.work = node.querySelector(".editor_work").value; 

            var select = node.querySelector(".editor_resources"); 
            task.resources = select.value ? [parseInt(select.value) || ""] : []; 

            let intensityInput = parseFloat(node.querySelector(".editor_intensity").value);
            task.intensity = validValues.find(value => Math.abs(value - intensityInput) < 0.01) || 1.00;

            task.planned_progress = parseInt(node.querySelector(".editor_planned_progress").value) || 100;

            return task.text; 
        },
        focus: function (node) { 
            node.querySelector(".editor_project").focus(); 
        } 
    };

    // Настройка разделов в окне добавления задачи
    gantt.config.lightbox.sections = [
        { name: "details", map_to: "text", type:"my_description", focus: true },
        { name: "time", type: "duration", map_to: "auto"}
    ];

    gantt.locale.labels.section_details = "";

    // Обновление часов
    gantt.attachEvent("onTaskUpdated", function(id, task) {
        gantt.refreshTask(id);
    })
    
    // Запрет создания связей между задачами
    gantt.attachEvent("onBeforeLinkAdd", function(id, link){
        return false;
    })

    // Получение начала и конца месяца
    function getMonthStartEnd(year, month) {
        var startDate = new Date(year, month, 1);
        var endDate = new Date(year, month + 1, 0); // Для получения последнего дня месяца используется день "0" следующего месяца
        return { start: startDate, end: endDate};
    }

    // Установка начальных параметров (текущий месяц)
    var currentDate = new Date();
    var currentYear = currentDate.getFullYear();
    var currentMonth = currentDate.getMonth();
    var dates = getMonthStartEnd(currentYear, currentMonth);

    // Установка текущего месяца по умолчанию в список
    document.getElementById("monthSelector").value = currentMonth;

    // Настройка диаграммы на выбарнный период
    gantt.config.start_date = dates.start;
    gantt.config.end_date = dates.end;
    gantt.config.min_date = dates.start;
    gantt.config.max_date = dates.end;

    // Обнолвние отображения диграммы на выбранный месяц
    function updateGanttMonth(year, month) {
        var dates = getMonthStartEnd(year, month);
        gantt.config.start_date = dates.start;
        gantt.config.end_date = dates.end;
        gantt.config.min_date = dates.start;
        gantt.config.max_date = dates.end;

        gantt.render();
    }

    // Обработчик для выбора месяца
    document.getElementById("monthSelector").addEventListener("change", function() {
        var selectedMonth = parseInt(this.value, 10);
        updateGanttMonth(currentYear, selectedMonth);
        updateTotalSummary();
    })

    // Обновление Итого планируемые ТЗ
    function updateTotalSummary() {
        let totalSum = 0;
        let selectedMonth = parseInt(document.getElementById("monthSelector").value, 10);
        let selectedYear = new Date().getFullYear();

        let monthStart = new Date(selectedYear, selectedMonth, 1);
        let monthEnd = new Date(selectedYear, selectedMonth + 1, 0);

        gantt.eachTask(function(task) {
            let taskStart = task.start_date;
            if (typeof taskStart === "String") {
                taskStart = gantt.date.str_to_date("%d-%m-%Y")(task.start_date);
            }

            if (taskStart >= monthStart && taskStart <= monthEnd) {
                totalSum += (task.duration || 0) * 8 * (task.intensity || 1.00);
            }
        })

        document.getElementById('total-sum').textContent = totalSum;
    }

    // Обновление Итого планируемые ТЗ при обновлении задачи
    gantt.attachEvent("onAfterTaskUpdate", function(id, task) {
        updateTotalSummary();
    })

    // При добавлении новой задачи
    gantt.attachEvent("onAfterTaskAdd", function(id, task) {
        updateTotalSummary();
    })

    // При удалении задачи
    gantt.attachEvent("onAfterTaskDelete", function(id, task) {
        updateTotalSummary();
    })

    gantt.init("GanttChart");

    gantt.parse({
        data: data,
        links: []
    });

    updateTotalSummary();
});
   


