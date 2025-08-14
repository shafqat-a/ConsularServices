/**
 * Optional: SurveyJS Creator Integration for Admin Users
 * This allows admins to create and modify survey forms through a visual interface
 */

class SurveyCreator {
    constructor() {
        this.creator = null;
        this.initializeCreator();
    }

    initializeCreator() {
        // Only load if user has admin privileges
        if (this.hasAdminAccess()) {
            this.loadCreatorScripts();
        }
    }

    hasAdminAccess() {
        // Check if user has admin access
        // You can implement this based on your authentication system
        return window.userRole === 'admin';
    }

    async loadCreatorScripts() {
        if (!window.SurveyCreator) {
            // Dynamically load SurveyJS Creator
            await this.loadScript('https://unpkg.com/survey-creator-knockout@latest/survey-creator-knockout.min.js');
            await this.loadStylesheet('https://unpkg.com/survey-creator-knockout@latest/survey-creator-knockout.min.css');
        }
    }

    loadScript(src) {
        return new Promise((resolve, reject) => {
            const script = document.createElement('script');
            script.src = src;
            script.onload = resolve;
            script.onerror = reject;
            document.head.appendChild(script);
        });
    }

    loadStylesheet(href) {
        return new Promise((resolve) => {
            const link = document.createElement('link');
            link.rel = 'stylesheet';
            link.href = href;
            link.onload = resolve;
            document.head.appendChild(link);
        });
    }

    openSurveyDesigner(containerId, existingModel = null) {
        if (!window.SurveyCreator) {
            console.error('SurveyJS Creator not loaded');
            return;
        }

        const options = {
            showLogicTab: true,
            showTranslationTab: true,
            showEmbededSurveyTab: false,
            allowModifyPages: true,
            showJSONEditorTab: true,
            showTestSurveyTab: true,
            showToolbox: true,
            showPropertyGrid: true
        };

        this.creator = new SurveyCreator.SurveyCreator(options);
        
        if (existingModel) {
            this.creator.text = JSON.stringify(existingModel);
        }

        // Save callback
        this.creator.saveSurveyFunc = (saveNo, callback) => {
            this.saveSurveyDesign(this.creator.text, callback);
        };

        // Render creator
        this.creator.render(containerId);
    }

    saveSurveyDesign(designJson, callback) {
        // Save the survey design to your backend
        fetch('/api/survey-design', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: designJson
        })
        .then(response => response.json())
        .then(result => {
            console.log('Survey design saved:', result);
            if (callback) callback(saveNo, true);
        })
        .catch(error => {
            console.error('Error saving survey design:', error);
            if (callback) callback(saveNo, false);
        });
    }
}

// Global instance for creator
window.surveyCreator = new SurveyCreator();
