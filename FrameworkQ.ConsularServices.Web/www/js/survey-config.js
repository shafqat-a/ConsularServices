/**
 * Modern SurveyJS Configuration and Utilities
 * Enhanced features for better UX and functionality
 */

class SurveyManager {
    constructor() {
        this.surveys = new Map();
        this.validators = new Map();
        this.themes = new Map();
        this.initializeGlobalSettings();
    }

    initializeGlobalSettings() {
        // Apply modern theme
        Survey.StylesManager.applyTheme("defaultV2");
        
        // Global survey settings
        Survey.settings.autoAdvanceDelay = 0;
        Survey.settings.allowShowingPreviewBeforeComplete = false;
        Survey.settings.showProgressBar = "top";
        Survey.settings.progressBarType = "buttons";
        
        // Custom CSS variables for your brand
        Survey.StylesManager.ThemeColors.default = {
            "--sjs-primary-backcolor": "#673ab0",
            "--sjs-primary-forecolor": "#fff",
            "--sjs-general-backcolor": "#f8f9fa",
            "--sjs-general-forecolor": "#333",
            "--sjs-border-default": "#e0e0e0"
        };
    }

    /**
     * Create an enhanced survey with modern features
     */
    createSurvey(containerId, model, options = {}) {
        const defaultOptions = {
            showNavigationButtons: true,
            showProgressBar: "top",
            questionsOnPageMode: "standard",
            checkErrorsMode: "onValueChanged",
            textUpdateMode: "onTyping",
            clearInvisibleValues: "onHidden",
            ...options
        };

        const survey = new Survey.Model(model);
        
        // Apply options
        Object.assign(survey, defaultOptions);

        // Add custom validation
        this.addCustomValidation(survey);
        
        // Add event handlers for better UX
        this.addEventHandlers(survey);
        
        // Render survey
        $(containerId).Survey({ model: survey });
        
        // Store reference
        this.surveys.set(containerId, survey);
        
        return survey;
    }

    /**
     * Enhanced validation with custom rules
     */
    addCustomValidation(survey) {
        survey.onValidateQuestion.add((sender, options) => {
            const question = options.question;
            
            // Email validation
            if (question.name === "email" && question.value) {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(question.value)) {
                    options.error = "Please enter a valid email address";
                }
            }
            
            // Custom password validation
            if (question.name === "password" && question.value) {
                if (question.value.length < 8) {
                    options.error = "Password must be at least 8 characters long";
                } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/.test(question.value)) {
                    options.error = "Password must contain uppercase, lowercase, and number";
                }
            }
            
            // Station name validation
            if (question.name === "stationName" && question.value) {
                if (question.value.length < 3) {
                    options.error = "Station name must be at least 3 characters";
                }
            }
        });
    }

    /**
     * Add enhanced event handlers
     */
    addEventHandlers(survey) {
        // Real-time data sync
        survey.onValueChanged.add((sender, options) => {
            this.onValueChanged(sender, options);
        });

        // Auto-save functionality
        survey.onPartialSend.add((sender, options) => {
            this.autoSave(sender, options);
        });

        // Enhanced completion handling
        survey.onComplete.add((sender) => {
            this.onSurveyComplete(sender);
        });

        // Progress tracking
        survey.onCurrentPageChanged.add((sender, options) => {
            this.trackProgress(sender, options);
        });
    }

    onValueChanged(sender, options) {
        // Real-time validation feedback
        console.log(`Field ${options.name} changed to: ${options.value}`);
        
        // Auto-format fields
        if (options.name === "email") {
            sender.setValue(options.name, options.value?.toLowerCase());
        }
    }

    autoSave(sender, options) {
        // Implement auto-save to prevent data loss
        const data = sender.data;
        localStorage.setItem(`survey_autosave_${sender.id}`, JSON.stringify(data));
        
        // Show save indicator
        this.showSaveIndicator("Auto-saved");
    }

    onSurveyComplete(sender) {
        const data = sender.data;
        console.log("Survey completed with data:", data);
        
        // Clear auto-save
        localStorage.removeItem(`survey_autosave_${sender.id}`);
        
        // Show completion message
        this.showCompletionMessage(sender);
    }

    trackProgress(sender, options) {
        const progress = Math.round((sender.currentPageNo + 1) / sender.pageCount * 100);
        console.log(`Survey progress: ${progress}%`);
    }

    /**
     * Enhanced model builder with modern question types
     */
    async buildEnhancedModel(objectType, existingData = null) {
        let model = {};

        switch (objectType) {
            case "user":
                model = {
                    title: "User Management",
                    description: "Manage user information and permissions",
                    logoPosition: "right",
                    completedHtml: "<h3>User information has been saved successfully!</h3>",
                    pages: [
                        {
                            name: "basicInfo",
                            title: "Basic Information",
                            elements: [
                                {
                                    type: "text",
                                    name: "email",
                                    title: "Email Address",
                                    isRequired: true,
                                    inputType: "email",
                                    placeholder: "user@example.com",
                                    description: "This will be used for login"
                                },
                                {
                                    type: "text",
                                    name: "firstName",
                                    title: "First Name",
                                    isRequired: true,
                                    placeholder: "Enter first name"
                                },
                                {
                                    type: "text",
                                    name: "lastName",
                                    title: "Last Name",
                                    isRequired: true,
                                    placeholder: "Enter last name"
                                }
                            ]
                        },
                        {
                            name: "security",
                            title: "Security Settings",
                            elements: [
                                {
                                    type: "text",
                                    name: "password",
                                    title: "Password",
                                    isRequired: true,
                                    inputType: "password",
                                    description: "Minimum 8 characters with uppercase, lowercase, and number"
                                },
                                {
                                    type: "dropdown",
                                    name: "role",
                                    title: "User Role",
                                    isRequired: true,
                                    choices: [
                                        { value: "admin", text: "Administrator" },
                                        { value: "operator", text: "Operator" },
                                        { value: "viewer", text: "Viewer" }
                                    ]
                                },
                                {
                                    type: "boolean",
                                    name: "isActive",
                                    title: "Account Active",
                                    defaultValue: true
                                }
                            ]
                        }
                    ],
                    showNavigationButtons: true,
                    showProgressBar: "top"
                };
                break;

            case "station":
                model = {
                    title: "Station Configuration",
                    description: "Configure station details and settings",
                    completedHtml: "<h3>Station has been configured successfully!</h3>",
                    elements: [
                        {
                            type: "text",
                            name: "stationName",
                            title: "Station Name",
                            isRequired: true,
                            placeholder: "Enter station name"
                        },
                        {
                            type: "dropdown",
                            name: "status",
                            title: "Station Status",
                            isRequired: true,
                            choices: [
                                { value: 0, text: "Inactive" },
                                { value: 1, text: "Active" },
                                { value: 2, text: "Maintenance" }
                            ]
                        },
                        {
                            type: "text",
                            name: "location",
                            title: "Location",
                            placeholder: "Physical location or description"
                        },
                        {
                            type: "rating",
                            name: "priority",
                            title: "Station Priority",
                            rateMin: 1,
                            rateMax: 5,
                            minRateDescription: "Low",
                            maxRateDescription: "High"
                        }
                    ]
                };
                break;

            case "service":
                model = {
                    title: "Service Configuration",
                    description: "Define service parameters and settings",
                    completedHtml: "<h3>Service has been configured successfully!</h3>",
                    elements: [
                        {
                            type: "text",
                            name: "serviceName",
                            title: "Service Name",
                            isRequired: true,
                            placeholder: "Enter service name"
                        },
                        {
                            type: "comment",
                            name: "description",
                            title: "Service Description",
                            placeholder: "Describe what this service does..."
                        },
                        {
                            type: "dropdown",
                            name: "category",
                            title: "Service Category",
                            choices: [
                                "Visa Services",
                                "Passport Services", 
                                "Notary Services",
                                "General Inquiry"
                            ]
                        },
                        {
                            type: "boolean",
                            name: "isActive",
                            title: "Service Active",
                            defaultValue: true
                        }
                    ]
                };
                break;

            default:
                // Dynamic model from API
                try {
                    const response = await fetch(`/api/typeinfo?itemtype=${encodeURIComponent(objectType)}`);
                    if (!response.ok) throw new Error(`Failed to fetch config for ${objectType}`);
                    model = await response.json();
                } catch (error) {
                    console.error(`Error building model for ${objectType}:`, error);
                    model = { title: "Error", elements: [] };
                }
                break;
        }

        // Restore existing data if provided
        if (existingData) {
            model.data = existingData;
        }

        return model;
    }

    /**
     * Enhanced grid/table functionality
     */
    createEnhancedTable(containerId, tableId, data, columns, actions, options = {}) {
        const defaultOptions = {
            sortable: true,
            filterable: true,
            pagination: true,
            pageSize: 10,
            responsive: true,
            ...options
        };

        // Clear container
        $(`#${containerId}`).empty();

        // Create wrapper with search and controls
        const wrapper = $(`<div class="table-wrapper"></div>`);
        
        if (defaultOptions.filterable) {
            const searchBox = $(`
                <div class="table-controls">
                    <input type="text" class="table-search" placeholder="Search..." />
                    <button class="btn-refresh">Refresh</button>
                </div>
            `);
            wrapper.append(searchBox);
        }

        const table = $(`<table class="enhanced-table" id="${tableId}"></table>`);
        const thead = $("<thead></thead>");
        const tbody = $("<tbody></tbody>");

        // Enhanced header with sorting
        const headerRow = $("<tr></tr>");
        columns.columns.forEach(column => {
            if (!column.hide) {
                const th = $(`<th data-column="${column.name}">${this.camelToProper(column.name)}</th>`);
                if (defaultOptions.sortable) {
                    th.addClass('sortable').on('click', () => this.sortTable(tableId, column.name));
                }
                headerRow.append(th);
            }
        });

        if (actions) {
            headerRow.append($("<th>Actions</th>"));
        }

        thead.append(headerRow);

        // Enhanced body with better formatting
        this.populateTableBody(tbody, data, columns, actions);

        table.append(thead).append(tbody);
        wrapper.append(table);

        // Add pagination if enabled
        if (defaultOptions.pagination) {
            const pagination = this.createPagination(data.length, defaultOptions.pageSize);
            wrapper.append(pagination);
        }

        $(`#${containerId}`).append(wrapper);

        // Add search functionality
        if (defaultOptions.filterable) {
            $(`#${containerId} .table-search`).on('input', (e) => {
                this.filterTable(tableId, e.target.value);
            });
        }
    }

    populateTableBody(tbody, data, columns, actions) {
        data.forEach(item => {
            const row = $("<tr></tr>");
            
            columns.columns.forEach(column => {
                if (!column.hide) {
                    const value = item[column.name];
                    const formattedValue = this.formatCellValue(value, column);
                    row.append($(`<td data-column="${column.name}">${formattedValue}</td>`));
                }
            });

            if (actions) {
                const actionCell = $("<td class='action-cell'></td>");
                actions.forEach(action => {
                    const button = $(`
                        <button class="btn btn-action btn-${action.type || 'primary'}" 
                                title="${action.title || action.label}">
                            ${action.icon ? `<i class="${action.icon}"></i>` : ''} ${action.label}
                        </button>
                    `).on('click', () => action.callback({ item }));
                    actionCell.append(button);
                });
                row.append(actionCell);
            }

            tbody.append(row);
        });
    }

    formatCellValue(value, column) {
        if (value === null || value === undefined) return '';
        
        // Format based on column type or name
        if (column.name.toLowerCase().includes('date')) {
            return new Date(value).toLocaleDateString();
        }
        if (column.name.toLowerCase().includes('status')) {
            return `<span class="status-badge status-${value.toLowerCase()}">${value}</span>`;
        }
        if (typeof value === 'boolean') {
            return value ? '✅' : '❌';
        }
        
        return value.toString();
    }

    createPagination(totalItems, pageSize) {
        const totalPages = Math.ceil(totalItems / pageSize);
        const pagination = $('<div class="pagination"></div>');
        
        for (let i = 1; i <= totalPages; i++) {
            const pageBtn = $(`<button class="page-btn ${i === 1 ? 'active' : ''}">${i}</button>`);
            pageBtn.on('click', () => this.goToPage(i, pageSize));
            pagination.append(pageBtn);
        }
        
        return pagination;
    }

    sortTable(tableId, columnName) {
        // Implement sorting logic
        console.log(`Sorting table ${tableId} by column ${columnName}`);
    }

    filterTable(tableId, searchTerm) {
        const table = $(`#${tableId}`);
        const rows = table.find('tbody tr');
        
        rows.each(function() {
            const row = $(this);
            const text = row.text().toLowerCase();
            row.toggle(text.includes(searchTerm.toLowerCase()));
        });
    }

    goToPage(pageNumber, pageSize) {
        console.log(`Going to page ${pageNumber} with page size ${pageSize}`);
        // Implement pagination logic
    }

    camelToProper(camelCaseStr) {
        if (!camelCaseStr) return "";
        return camelCaseStr
            .replace(/([a-z])([A-Z])/g, '$1 $2')
            .replace(/\b\w/g, char => char.toUpperCase());
    }

    showSaveIndicator(message) {
        // Show a temporary save indicator
        const indicator = $(`<div class="save-indicator">${message}</div>`);
        $('body').append(indicator);
        setTimeout(() => indicator.fadeOut(() => indicator.remove()), 2000);
    }

    showCompletionMessage(survey) {
        // Custom completion handling
        console.log("Survey completed successfully");
    }

    // Analytics and insights
    trackSurveyUsage(surveyType, action) {
        // Track survey interactions for analytics
        console.log(`Survey Analytics: ${surveyType} - ${action}`);
    }
}

// Global instance
window.surveyManager = new SurveyManager();
