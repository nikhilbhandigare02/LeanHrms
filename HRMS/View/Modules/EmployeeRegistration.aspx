<%@ Page Title="Employee Registration" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="EmployeeRegistration.aspx.cs" Inherits="HRMS.View.Modules.EmployeeRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <style>
        :root {
            --onboarding-blue: #2563EB;
            --onboarding-blue-dark: #1D4ED8;
            --onboarding-ink: #172033;
            --onboarding-muted: #64748B;
            --onboarding-line: #E5E7EB;
            --onboarding-bg: #F8FAFC;
            --onboarding-card: #FFFFFF;
            --onboarding-success: #16A34A;
        }

        body {
            background: var(--onboarding-bg) !important;
        }

        .onboarding-page {
            color: var(--onboarding-ink);
            max-width: 1440px;
            margin: 0 auto;
            padding-bottom: 32px;
        }

        .onboarding-top {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 20px;
            margin-bottom: 22px;
        }

        .onboarding-eyebrow {
            color: var(--onboarding-muted);
            font-size: 13px;
            font-weight: 600;
            margin-bottom: 8px;
        }

        .onboarding-eyebrow span {
            color: var(--onboarding-blue);
        }

        .onboarding-title {
            font-size: 28px;
            font-weight: 800;
            letter-spacing: 0;
            margin: 0;
        }

        .onboarding-subtitle {
            color: var(--onboarding-muted);
            font-size: 14px;
            margin: 8px 0 0;
        }

        .onboarding-status-card,
        .onboarding-card,
        .success-card {
            background: var(--onboarding-card);
            border: 1px solid rgba(226, 232, 240, .9);
            border-radius: 12px;
            box-shadow: 0 18px 45px rgba(15, 23, 42, .07);
        }

        .onboarding-status-card {
            min-width: 260px;
            padding: 16px 18px;
        }

        .status-label {
            color: var(--onboarding-muted);
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .status-value {
            display: flex;
            align-items: center;
            gap: 9px;
            font-weight: 800;
            margin-top: 8px;
        }

        .status-dot {
            width: 10px;
            height: 10px;
            border-radius: 50%;
            background: var(--onboarding-success);
            box-shadow: 0 0 0 5px rgba(22, 163, 74, .12);
        }

        .wizard-shell {
            display: block;
        }

        .step-panel {
            background: #FFFFFF;
            border-radius: 12px;
            box-shadow: 0 14px 36px rgba(15, 23, 42, .06);
            border: 1px solid var(--onboarding-line);
            padding: 14px;
            margin-bottom: 18px;
            display: grid;
            grid-template-columns: repeat(6, minmax(125px, 1fr));
            gap: 10px;
            overflow-x: auto;
        }

        .step-item {
            display: flex;
            flex-direction: column;
            gap: 8px;
            align-items: center;
            justify-content: center;
            min-height: 88px;
            padding: 12px 8px;
            border-radius: 12px;
            color: var(--onboarding-muted);
            cursor: pointer;
            text-align: center;
            border: 1px solid transparent;
        }

        .step-number {
            width: 32px;
            height: 32px;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            background: #EFF6FF;
            color: var(--onboarding-blue);
            font-weight: 800;
            font-size: 13px;
        }

        .step-title {
            font-size: 12px;
            font-weight: 700;
            line-height: 1.25;
        }

        .step-item.active {
            background: #EFF6FF;
            color: var(--onboarding-blue);
            border-color: #BFDBFE;
        }

        .step-item.active .step-number {
            background: var(--onboarding-blue);
            color: #FFFFFF;
        }

        .step-item.done .step-number {
            background: #DCFCE7;
            color: var(--onboarding-success);
        }

        .onboarding-card {
            padding: 26px;
            min-height: 650px;
        }

        .card-heading {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 18px;
            border-bottom: 1px solid var(--onboarding-line);
            padding-bottom: 18px;
            margin-bottom: 22px;
        }

        .card-heading h2 {
            font-size: 21px;
            font-weight: 800;
            margin: 0;
        }

        .card-heading p {
            color: var(--onboarding-muted);
            font-size: 13px;
            margin: 7px 0 0;
        }

        .step-pill {
            color: var(--onboarding-blue);
            background: #EFF6FF;
            border-radius: 999px;
            padding: 8px 12px;
            font-size: 12px;
            font-weight: 800;
            white-space: nowrap;
        }

        .form-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 18px 20px;
        }

        .form-group {
            margin: 0;
        }

        .form-group label {
            color: #334155;
            display: block;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .form-control-modern,
        .form-select-modern {
            width: 100%;
            height: 44px;
            border: 1px solid #D6DEE9;
            border-radius: 10px;
            background: #FFFFFF;
            color: var(--onboarding-ink);
            font-size: 14px;
            padding: 10px 12px;
            transition: border-color .18s ease, box-shadow .18s ease;
        }

        .form-control-modern:focus,
        .form-select-modern:focus {
            border-color: var(--onboarding-blue);
            box-shadow: 0 0 0 4px rgba(37, 99, 235, .12);
            outline: none;
        }

        .mobile-select-wrap {
            position: relative;
            width: 100%;
        }

        .mobile-select-trigger,
        .mobile-select-menu {
            display: none;
        }

        .toggle-field {
            align-items: center;
            background: transparent;
            border: 0;
            border-radius: 0;
            display: flex;
            gap: 10px;
            height: 44px;
            justify-content: flex-start;
            padding: 0;
        }

        .toggle-field:hover {
            background: transparent;
            box-shadow: none;
        }

        .switch-option {
            color: #64748B;
            font-size: 13px;
            font-weight: 800;
            min-width: 24px;
        }

        .toggle-switch {
            display: inline-flex;
            position: relative;
        }

        .toggle-switch input {
            height: 0;
            opacity: 0;
            position: absolute;
            width: 0;
        }

        .toggle-slider {
            background: #E2E8F0;
            border: 1px solid #CBD5E1;
            border-radius: 999px;
            cursor: pointer;
            display: inline-block;
            flex: 0 0 54px;
            height: 30px;
            position: relative;
            transition: background-color .18s ease, border-color .18s ease, box-shadow .18s ease;
            width: 54px;
        }

        .toggle-slider:before {
            background: #FFFFFF;
            border-radius: 50%;
            box-shadow: 0 2px 6px rgba(15, 23, 42, .2);
            content: "";
            height: 24px;
            left: 2px;
            position: absolute;
            top: 2px;
            transition: transform .18s ease;
            width: 24px;
        }

        .toggle-switch input:checked + .toggle-slider {
            background: var(--onboarding-blue);
            border-color: var(--onboarding-blue);
            box-shadow: 0 0 0 4px rgba(37, 99, 235, .12);
        }

        .toggle-switch input:checked + .toggle-slider:before {
            transform: translateX(26px);
        }

        .premium-grid {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 16px;
        }

        .premium-field {
            background: #F8FAFC;
            border: 1px solid #E2E8F0;
            border-radius: 12px;
            padding: 14px;
        }

        .premium-field label {
            color: var(--onboarding-muted);
            font-size: 12px;
        }

        .setup-band {
            background: linear-gradient(180deg, #FFFFFF 0%, #F8FAFC 100%);
            border: 1px solid #E2E8F0;
            border-radius: 12px;
            padding: 18px;
        }

        .asset-toolbar {
            display: flex;
            justify-content: flex-end;
            margin-bottom: 14px;
        }

        .table-responsive {
            overflow-x: auto;
            padding-bottom: 2px;
        }

        .asset-table {
            width: 100%;
            min-width: 1120px;
            border-collapse: separate;
            border-spacing: 0;
            overflow: hidden;
            border: 1px solid var(--onboarding-line);
            border-radius: 12px;
            table-layout: fixed;
        }

        .asset-table th,
        .asset-table td {
            padding: 16px 14px;
            border-bottom: 1px solid var(--onboarding-line);
            font-size: 13px;
            vertical-align: middle;
        }

        .asset-table th {
            background: #F8FAFC;
            color: #475569;
            font-weight: 800;
        }

        .asset-table th:nth-child(1),
        .asset-table td:nth-child(1) {
            width: 150px;
        }

        .asset-table th:nth-child(2),
        .asset-table td:nth-child(2),
        .asset-table th:nth-child(3),
        .asset-table td:nth-child(3) {
            width: 160px;
        }

        .asset-table th:nth-child(4),
        .asset-table td:nth-child(4),
        .asset-table th:nth-child(5),
        .asset-table td:nth-child(5) {
            width: 170px;
        }

        .asset-table th:nth-child(6),
        .asset-table td:nth-child(6),
        .asset-table th:nth-child(7),
        .asset-table td:nth-child(7) {
            width: 150px;
        }

        .asset-table th:nth-child(8),
        .asset-table td:nth-child(8) {
            width: 86px;
            text-align: center;
        }

        .asset-table .form-control-modern,
        .asset-table .form-select-modern {
            min-width: 0;
            width: 100%;
        }

        .asset-delete-btn {
            align-items: center;
            background: #FFF1F2;
            border: 1px solid #FFE4E6;
            border-radius: 8px;
            color: #E11D48;
            display: inline-flex;
            height: 36px;
            justify-content: center;
            transition: background .18s ease, border-color .18s ease, transform .18s ease;
            width: 36px;
        }

        .asset-delete-btn:hover {
            background: #FFE4E6;
            border-color: #FDA4AF;
            transform: translateY(-1px);
        }

        .asset-table tr:last-child td {
            border-bottom: 0;
        }

        .badge-soft {
            border-radius: 999px;
            display: inline-flex;
            padding: 5px 10px;
            font-size: 12px;
            font-weight: 800;
            background: #EFF6FF;
            color: var(--onboarding-blue);
        }

        .review-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 16px;
        }

        .review-card {
            border: 1px solid #E2E8F0;
            border-radius: 12px;
            padding: 18px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 12px;
            background: #FFFFFF;
        }

        .review-name {
            display: flex;
            align-items: center;
            gap: 10px;
            font-weight: 800;
        }

        .review-check {
            width: 28px;
            height: 28px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            background: #DCFCE7;
            color: var(--onboarding-success);
            font-weight: 900;
        }

        .wizard-actions {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 12px;
            margin-top: 26px;
            border-top: 1px solid var(--onboarding-line);
            padding-top: 20px;
        }

        .action-left,
        .action-right {
            display: flex;
            gap: 10px;
        }

        .btn-modern {
            border: 0;
            border-radius: 10px;
            min-height: 42px;
            padding: 10px 18px;
            font-weight: 800;
            font-size: 14px;
            cursor: pointer;
        }

        .btn-primary-modern {
            background: var(--onboarding-blue);
            color: #FFFFFF;
            box-shadow: 0 10px 20px rgba(37, 99, 235, .22);
        }

        .btn-primary-modern:hover {
            background: var(--onboarding-blue-dark);
        }

        .btn-secondary-modern {
            background: #FFFFFF;
            color: #334155;
            border: 1px solid #CBD5E1;
        }

        .btn-ghost-modern {
            background: transparent;
            color: var(--onboarding-muted);
        }

        .wizard-step,
        .success-card {
            display: none;
        }

        .wizard-step.active,
        .success-card.active {
            display: block;
        }

        .success-card {
            padding: 42px;
            text-align: center;
        }

        .success-illustration {
            width: 180px;
            height: 180px;
            border-radius: 50%;
            margin: 0 auto 24px;
            display: flex;
            align-items: center;
            justify-content: center;
            background: radial-gradient(circle at 50% 42%, #FFFFFF 0 31%, #DBEAFE 32% 56%, #EFF6FF 57% 100%);
            border: 1px solid #BFDBFE;
            box-shadow: 0 22px 48px rgba(37, 99, 235, .18);
        }

        .success-illustration span {
            width: 78px;
            height: 78px;
            border-radius: 50%;
            background: var(--onboarding-success);
            color: #FFFFFF;
            font-size: 42px;
            font-weight: 900;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .success-card h2 {
            font-size: 28px;
            font-weight: 900;
            margin: 0 0 10px;
        }

        .credential-box {
            display: grid;
            grid-template-columns: repeat(3, minmax(0, 1fr));
            gap: 14px;
            margin: 28px auto 20px;
            max-width: 820px;
        }

        .credential-item {
            text-align: left;
            border: 1px solid #E2E8F0;
            border-radius: 12px;
            padding: 16px;
            background: #F8FAFC;
        }

        .credential-label {
            color: var(--onboarding-muted);
            font-size: 12px;
            font-weight: 800;
            text-transform: uppercase;
        }

        .credential-value {
            color: var(--onboarding-ink);
            font-size: 17px;
            font-weight: 900;
            margin-top: 6px;
        }

        .send-mail {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            font-weight: 700;
            color: #334155;
            margin-bottom: 26px;
        }

        @media (max-width: 1199px) {
            .premium-grid,
            .credential-box {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 767px) {
            .onboarding-top,
            .card-heading,
            .wizard-actions {
                align-items: stretch;
                flex-direction: column;
            }

            .onboarding-status-card {
                min-width: 0;
            }

            .form-grid,
            .premium-grid,
            .review-grid,
            .credential-box {
                grid-template-columns: 1fr;
            }

            .onboarding-card,
            .success-card {
                padding: 20px;
            }
        }

        .onboarding-page {
            max-width: 1360px;
        }

        .onboarding-top {
            align-items: flex-start;
            display: block;
            margin-bottom: 18px;
        }

        .onboarding-title {
            font-size: 22px;
        }

        .onboarding-subtitle {
            color: #4B5B73;
            font-size: 13px;
            font-weight: 600;
        }

        .onboarding-status-card,
        .step-panel,
        .step-pill,
        #nextButton {
            display: none !important;
        }

        .onboarding-card {
            background: transparent;
            border: 0;
            border-radius: 0;
            box-shadow: none;
            min-height: 0;
            padding: 0;
        }

        .wizard-step {
            background: #FFFFFF;
            border: 1px solid #DFE7F2;
            border-radius: 8px;
            box-shadow: 0 12px 34px rgba(15, 23, 42, .05);
            display: block;
            margin-bottom: 10px;
            overflow: hidden;
            transition: border-color .18s ease, box-shadow .18s ease;
        }

        .wizard-step:hover {
            border-color: #BFD2F6;
            box-shadow: 0 14px 32px rgba(37, 99, 235, .08);
        }

        .wizard-step.collapsed > .form-grid,
        .wizard-step.collapsed > .premium-grid,
        .wizard-step.collapsed > .setup-band,
        .wizard-step.collapsed > .asset-toolbar,
        .wizard-step.collapsed > .table-responsive,
        .wizard-step.collapsed > .review-grid,
        .wizard-step.collapsed > .section-save-row {
            display: none;
        }

        .card-heading {
            align-items: center;
            border-bottom: 1px solid #E8EEF6;
            cursor: pointer;
            display: flex;
            gap: 12px;
            margin: 0;
            padding: 16px 22px;
            user-select: none;
        }

        .wizard-step.collapsed .card-heading {
            border-bottom: 0;
        }

        .card-heading > div {
            align-items: center;
            display: flex;
            gap: 14px;
            min-width: 0;
        }

        .card-heading h2 {
            color: #172033;
            font-size: 15px;
            font-weight: 900;
            margin: 0;
        }

        .card-heading p {
            display: none;
        }

        .registration-section-status {
            align-items: center;
            display: inline-flex;
            gap: 6px;
            justify-content: flex-end;
            margin-left: auto;
            min-width: 78px;
            white-space: nowrap;
            color: #F97316;
            font-size: 12px;
            font-weight: 900;
        }

        .registration-section-status.saved {
            color: #0F9F6E;
        }

        .registration-section-status.neutral {
            color: #64748B;
        }

        .registration-section-status i {
            font-size: 12px;
        }

        .registration-chevron {
            color: #1E3A8A;
            font-size: 13px;
            transition: transform .18s ease;
        }

        .wizard-step:not(.collapsed) .registration-chevron {
            transform: rotate(180deg);
        }

        .accordion-number {
            align-items: center;
            background: var(--onboarding-blue);
            border-radius: 50%;
            color: #FFFFFF;
            display: inline-flex;
            flex: 0 0 28px;
            font-size: 13px;
            font-weight: 900;
            height: 28px;
            justify-content: center;
            width: 28px;
        }

        .wizard-step > .form-grid,
        .wizard-step > .premium-grid,
        .wizard-step > .setup-band,
        .wizard-step > .review-grid {
            padding: 22px 28px 20px;
        }

        .wizard-step > .asset-toolbar {
            padding: 18px 28px 0;
        }

        .wizard-step > .table-responsive {
            padding: 14px 28px 22px;
        }

        .setup-band {
            background: #FFFFFF;
            border: 0;
            border-radius: 0;
        }

        .form-grid {
            grid-template-columns: repeat(5, minmax(0, 1fr));
            gap: 18px 28px;
        }

        .premium-grid {
            grid-template-columns: repeat(4, minmax(0, 1fr));
        }

        .premium-field {
            background: transparent;
            border: 0;
            border-radius: 0;
            padding: 0;
        }

        .form-group label,
        .premium-field label {
            color: #24324A;
            font-size: 12px;
            font-weight: 800;
        }

        .is-required:after {
            color: #E11D48;
            content: " *";
        }

        .form-control-modern,
        .form-select-modern {
            border-radius: 5px;
            font-size: 13px;
            height: 38px;
            padding: 8px 10px;
        }

        .validation-error {
            border-color: #E11D48 !important;
            box-shadow: 0 0 0 3px rgba(225, 29, 72, .12) !important;
        }

        .validation-message {
            color: #E11D48;
            display: none;
            font-size: 11px;
            font-weight: 800;
            margin-top: 5px;
        }

        .validation-error + .validation-message {
            display: block;
        }

        .section-save-row {
            display: flex;
            justify-content: flex-end;
            padding: 0 28px 24px;
        }

        .wizard-actions {
            border-top: 1px solid #DDE7F3;
            justify-content: flex-end;
            margin-top: 18px;
            padding-top: 20px;
        }

        .btn-modern {
            align-items: center;
            display: inline-flex;
            justify-content: center;
            min-width: 126px;
        }

        #submitButton {
            display: inline-flex !important;
        }

        @media (max-width: 1199px) {
            .form-grid,
            .premium-grid {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (max-width: 767px) {
            html,
            body {
                max-width: 100%;
                overflow-x: hidden;
            }

            .page-content,
            .container-fluid {
                max-width: 100% !important;
                overflow-x: hidden;
                padding-left: 10px !important;
                padding-right: 10px !important;
                width: 100% !important;
            }

            .onboarding-page {
                max-width: 100% !important;
                overflow: visible;
                width: 100%;
            }

            .form-grid,
            .premium-grid {
                grid-template-columns: 1fr;
            }

            .form-group,
            .premium-field {
                min-width: 0;
                width: 100%;
            }

            .form-control-modern,
            .form-select-modern,
            .form-select-modern option {
                box-sizing: border-box;
                max-width: calc(100vw - 52px);
                min-width: 0;
                width: 100%;
            }

            .form-select-modern option {
                white-space: normal;
            }

            .mobile-select-wrap {
                max-width: 100%;
                position: relative;
                width: 100%;
            }

            .mobile-select-native {
                display: none !important;
            }

            .mobile-select-trigger {
                align-items: center;
                background: #FFFFFF;
                border: 1px solid #D6DEE9;
                border-radius: 5px;
                color: var(--onboarding-ink);
                display: flex;
                font-size: 13px;
                height: 38px;
                justify-content: space-between;
                max-width: 100%;
                padding: 8px 10px;
                text-align: left;
                width: 100%;
            }

            .mobile-select-trigger:after {
                border-color: #0F172A transparent transparent transparent;
                border-style: solid;
                border-width: 5px 4px 0 4px;
                content: "";
                flex: 0 0 auto;
                margin-left: 10px;
            }

            .mobile-select-trigger[disabled] {
                background: #F8FAFC;
                color: #94A3B8;
                cursor: not-allowed;
            }

            .mobile-select-native.validation-error + .mobile-select-trigger {
                border-color: #E11D48 !important;
                box-shadow: 0 0 0 3px rgba(225, 29, 72, .12) !important;
            }

            .mobile-select-menu {
                background: #FFFFFF;
                border: 1px solid #CBD5E1;
                border-radius: 0 0 6px 6px;
                box-sizing: border-box;
                box-shadow: 0 16px 34px rgba(15, 23, 42, .14);
                display: none;
                left: 0;
                list-style: none;
                margin: -1px 0 0;
                max-height: 220px;
                max-width: 100% !important;
                min-width: 0 !important;
                overflow-y: auto;
                overflow-x: hidden;
                padding: 5px;
                position: absolute;
                right: auto;
                top: 100%;
                width: 100%;
                z-index: 9999;
            }

            .mobile-select-wrap.open .mobile-select-trigger {
                border-color: #CBD5E1;
                border-radius: 5px 5px 0 0;
                box-shadow: none;
            }

            .mobile-select-wrap.open .mobile-select-menu {
                display: block;
            }

            .mobile-select-option {
                border-radius: 5px;
                color: #0F172A;
                cursor: pointer;
                font-size: 13px;
                line-height: 1.3;
                max-width: 100%;
                padding: 10px 12px;
                white-space: normal;
                word-break: break-word;
            }

            .mobile-select-option:hover,
            .mobile-select-option.selected {
                background: #EFF6FF;
                color: var(--onboarding-blue);
                font-weight: 800;
            }

            .mobile-select-option.disabled {
                color: #94A3B8;
                cursor: default;
                font-weight: 600;
            }

            .card-heading {
                flex-direction: row;
                padding: 14px 16px;
            }

            .card-heading:after {
                font-size: 11px;
            }

            .wizard-step > .form-grid,
            .wizard-step > .premium-grid,
            .wizard-step > .setup-band,
            .wizard-step > .review-grid,
            .wizard-step > .table-responsive {
                padding-left: 16px;
                padding-right: 16px;
            }

            .wizard-step {
                overflow: visible;
                width: 100%;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="onboarding-page">
        <div class="onboarding-top">
            <div>
                <div class="onboarding-eyebrow">Employee Onboarding <span>&gt;</span> Employee Registration</div>
                <h1 class="onboarding-title">Employee Registration</h1>
                
            </div>
            <div class="onboarding-status-card">
                <div class="status-label">Onboarding Status</div>
                <div class="status-value"><span class="status-dot"></span><span id="statusText">Draft in progress</span></div>
            </div>
        </div>

        <div id="wizardArea" class="wizard-shell">
            <aside class="step-panel" aria-label="Employee registration steps">
                <div class="step-item active" data-step-link="1"><span class="step-number">1</span><span class="step-title">Basic Employee Information</span></div>
                <div class="step-item" data-step-link="2"><span class="step-number">2</span><span class="step-title">Employment Information</span></div>
                <div class="step-item" data-step-link="3"><span class="step-number">3</span><span class="step-title">Organization Details</span></div>
                <div class="step-item" data-step-link="4"><span class="step-number">4</span><span class="step-title">Attendance &amp; Shift Information</span></div>
                <div class="step-item" data-step-link="5"><span class="step-number">5</span><span class="step-title">Assets Assignment</span></div>
                <div class="step-item" data-step-link="6"><span class="step-number">6</span><span class="step-title">Review &amp; Submit</span></div>
            </aside>

            <main class="onboarding-card">
                <asp:HiddenField ID="hdnAssetsJson" runat="server" ClientIDMode="Static" />
                <section class="wizard-step active pending" data-step="1">
                    <div class="card-heading">
                        <div><span class="accordion-number">1</span><h2>Basic Information</h2><p>Capture the employee identity and primary contact details.</p></div>
                        <span class="registration-section-status"><i class="far fa-clock"></i> Pending</span>
                        <i class="fas fa-chevron-down registration-chevron"></i>
                        <span class="step-pill">Step 1 of 6</span>
                    </div>
                    <div class="form-grid">
                        <div class="form-group"><label class="is-required">Employee Code</label><input id="txtEmployeeCode" runat="server" class="form-control-modern" placeholder="Enter employee code" /><span id="employeeCodeDuplicateMessage" class="validation-message"></span></div>
                        <div class="form-group"><label class="is-required">Username</label><input id="txtUsername" runat="server" class="form-control-modern" placeholder="Enter username" /><span id="usernameDuplicateMessage" class="validation-message"></span></div>
                        <div class="form-group"><label class="is-required">First Name</label><input id="txtFirstName" runat="server" class="form-control-modern" placeholder="Enter first name" /><span id="firstNameValidationMessage" class="validation-message"></span></div>
                        <div class="form-group"><label>Middle Name</label><input id="txtMiddleName" runat="server" class="form-control-modern" placeholder="Enter middle name" /><span id="middleNameValidationMessage" class="validation-message"></span></div>
                        <div class="form-group"><label class="is-required">Last Name</label><input id="txtLastName" runat="server" class="form-control-modern" placeholder="Enter last name" /><span id="lastNameValidationMessage" class="validation-message"></span></div>
                        <div class="form-group"><label class="is-required">Email ID</label><input id="txtEmail" runat="server" class="form-control-modern" type="email" placeholder="name@company.com" /><span id="emailDuplicateMessage" class="validation-message"></span></div>
                        <div class="form-group"><label class="is-required">Mobile Number</label><input id="txtMobileNumber" runat="server" class="form-control-modern" placeholder="Enter mobile number" inputmode="numeric" maxlength="10" /><span id="mobileDuplicateMessage" class="validation-message"></span></div>
                        <div class="form-group"><label class="is-required">Gender</label><asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="form-group"><label class="is-required">Date of Birth</label><input id="txtDateOfBirth" runat="server" class="form-control-modern" type="date" /></div>
                        <div class="form-group"><label class="is-required">Nationality</label><asp:DropDownList ID="ddlNationality" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="form-group"><label class="is-required">Marital Status</label><asp:DropDownList ID="ddlMaritalStatus" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="form-group"><label class="is-required">Blood Group</label><asp:DropDownList ID="ddlBloodGroup" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                    </div>
                    <div class="section-save-row"><button type="button" class="btn-modern btn-secondary-modern" data-collapse-step="1">Save</button></div>
                </section>
                <%--//////--%>
                <section class="wizard-step collapsed pending" data-step="2">
                    <div class="card-heading">
                        <div><span class="accordion-number">2</span><h2>Employment Information</h2><p>Set the employee lifecycle, joining, probation, and separation attributes.</p></div>
                        <span class="registration-section-status"><i class="far fa-clock"></i> Pending</span>
                        <i class="fas fa-chevron-down registration-chevron"></i>
                        <span class="step-pill">Step 2 of 6</span>
                    </div>
                    <div class="form-grid">
                        <div class="form-group"><label class="is-required">Employment Type</label><asp:DropDownList ID="ddlEmploymentType" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="form-group"><label class="is-required">Employee Category</label><asp:DropDownList ID="ddlEmployeeCategory" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="form-group"><label class="is-required">Joining Date</label><input id="txtJoiningDate" runat="server" class="form-control-modern" type="date" /></div>
                        <div class="form-group"><label class="is-required">Probation Period</label><asp:DropDownList ID="ddlProbationPeriod" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="form-group"><label>Confirmation Date</label><input id="txtConfirmationDate" runat="server" class="form-control-modern" type="date" disabled /></div>
                        <div class="form-group"><label>Probation End Date</label><input id="txtProbationEndDate" runat="server" class="form-control-modern" type="date" disabled /></div>
                        <div class="form-group"><label class="is-required">Employee Status</label><asp:DropDownList ID="ddlEmployeeStatus" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="form-group"><label>Notice Period</label><input id="txtNoticePeriod" runat="server" class="form-control-modern" placeholder="Enter notice period days" /></div>
                        <div class="form-group"><label>Exit Date</label><input id="txtExitDate" runat="server" class="form-control-modern" type="date" disabled /></div>
                        <div class="form-group"><label>Separation Reason</label><asp:DropDownList ID="ddlSeparationReason" runat="server" CssClass="form-select-modern" Enabled="false"></asp:DropDownList></div>
                    </div>
                    <div class="section-save-row"><button type="button" class="btn-modern btn-secondary-modern" data-collapse-step="2">Save</button></div>
                </section>

                <section class="wizard-step collapsed pending" data-step="3">
                    <div class="card-heading">
                        <div><span class="accordion-number">3</span><h2>Organization Details</h2><p>Map the employee into company structure, reporting hierarchy, and level.</p></div>
                        <span class="registration-section-status"><i class="far fa-clock"></i> Pending</span>
                        <i class="fas fa-chevron-down registration-chevron"></i>
                        <span class="step-pill">Step 3 of 6</span>
                    </div>
                    <div class="premium-grid">
                        <div class="premium-field"><label class="is-required">Company</label><asp:DropDownList ID="ddlCompany" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="premium-field"><label class="is-required">Department</label><asp:DropDownList ID="ddlDepartment" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="premium-field"><label class="is-required">Branch Office</label><input id="txtBranchOffice" runat="server" class="form-control-modern" placeholder="Enter branch office" /></div>
                        <div class="premium-field"><label class="is-required">Location</label><input id="txtLocation" runat="server" class="form-control-modern" placeholder="Enter location" /></div>
                        <div class="premium-field"><label class="is-required">Designation</label><asp:DropDownList ID="ddlDesignation" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="premium-field"><label class="is-required">Reporting Manager</label><asp:DropDownList ID="ddlReportingManager" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="premium-field"><label class="is-required">Functional Manager</label><asp:DropDownList ID="ddlFunctionalManager" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="premium-field"><label class="is-required">HOD</label><asp:DropDownList ID="ddlHod" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        <div class="premium-field"><label>Employee Level</label><asp:DropDownList ID="ddlEmployeeLevel" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                    </div>
                    <div class="section-save-row"><button type="button" class="btn-modern btn-secondary-modern" data-collapse-step="3">Save</button></div>
                </section>

                <section class="wizard-step collapsed pending" data-step="4">
                    <div class="card-heading">
                        <div><span class="accordion-number">4</span><h2>Attendance Information</h2><p>Configure work schedule, attendance rules, device mapping, and location policy.</p></div>
                        <span class="registration-section-status"><i class="far fa-clock"></i> Pending</span>
                        <i class="fas fa-chevron-down registration-chevron"></i>
                        <span class="step-pill">Step 4 of 6</span>
                    </div>
                    <div class="setup-band">
                        <div class="form-grid">
                            <div class="form-group"><label class="is-required">Attendance Type</label><asp:DropDownList ID="ddlAttendanceType" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                            <div class="form-group"><label class="is-required">Weekly Off</label><asp:DropDownList ID="ddlWeeklyOff" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                            <div class="form-group"><label class="is-required">Working Hours</label><input id="txtWorkingHours" runat="server" class="form-control-modern" placeholder="HH:mm" inputmode="numeric" /><span id="workingHoursValidationMessage" class="validation-message"></span></div>
                            <div class="form-group"><label class="is-required">Attendance Policy</label><asp:DropDownList ID="ddlAttendancePolicy" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                            <div class="form-group"><label class="is-required">Punching Device ID</label><input id="txtPunchingDeviceId" runat="server" class="form-control-modern" placeholder="Device ID" /></div>
                            <div class="form-group"><label class="is-required">Biometric ID</label><input id="txtBiometricId" runat="server" class="form-control-modern" placeholder="Biometric ID" /></div>
                            <div class="form-group">
                                <label class="is-required">Overtime Eligible</label>
                                <div class="toggle-field">
                                    <span class="switch-option">No</span>
                                    <label class="toggle-switch">
                                        <input id="chkOvertimeEligible" runat="server" type="checkbox" />
                                        <span class="toggle-slider"></span>
                                    </label>
                                    <span class="switch-option">Yes</span>
                                </div>
                            </div>
                            <div class="form-group"><label id="overtimeRateLabel">Overtime Rate</label><input id="txtOvertimeRate" runat="server" class="form-control-modern" placeholder="Per hour rate" disabled /></div>
                            <div class="form-group"><label class="is-required">Work Location</label><asp:DropDownList ID="ddlWorkLocation" runat="server" CssClass="form-select-modern"></asp:DropDownList></div>
                        </div>
                    </div>
                    <div class="section-save-row"><button type="button" class="btn-modern btn-secondary-modern" data-collapse-step="4">Save</button></div>
                </section>

                <section class="wizard-step collapsed pending" data-step="5">
                    <div class="card-heading">
                        <div><span class="accordion-number">5</span><h2>Assets Assignment</h2><p>Allocate company assets and track return status from day one.</p></div>
                        <span class="registration-section-status"><i class="far fa-clock"></i> Pending</span>
                        <i class="fas fa-chevron-down registration-chevron"></i>
                        <span class="step-pill">Step 5 of 6</span>
                    </div>
                    <div class="asset-toolbar">
                        <button type="button" id="assignAssetButton" class="btn-modern btn-primary-modern">Assign Asset</button>
                    </div>
                    <div class="table-responsive">
                        <table class="asset-table">
                            <thead><tr><th class="is-required">Asset Type</th><th class="is-required">Asset Number</th><th class="is-required">Asset Name</th><th class="is-required">Assigned Date</th><th>Return Date</th><th class="is-required">Asset Condition</th><th class="is-required">Asset Status</th><th>Action</th></tr></thead>
                            <tbody id="assetRows">
                                <tr class="asset-row">
                                    <td><input id="txtAssetType" runat="server" class="form-control-modern" placeholder="Enter asset type" /></td>
                                    <td><input id="txtAssetNumber" runat="server" class="form-control-modern" /></td>
                                    <td><input id="txtAssetName" runat="server" class="form-control-modern" /></td>
                                    <td><input id="txtAssignedDate" runat="server" class="form-control-modern" type="date" /></td>
                                    <td><input id="txtReturnDate" runat="server" class="form-control-modern" type="date" /></td>
                                    <td><asp:DropDownList ID="ddlAssetCondition" runat="server" CssClass="form-select-modern"></asp:DropDownList></td>
                                    <td><asp:DropDownList ID="ddlAssetStatus" runat="server" CssClass="form-select-modern"></asp:DropDownList></td>
                                    <td></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                    <div class="section-save-row"><button type="button" class="btn-modern btn-secondary-modern" data-collapse-step="5">Save</button></div>
                </section>

                <section class="wizard-step collapsed pending-neutral" data-step="6">
                    <div class="card-heading">
                        <div><span class="accordion-number">6</span><h2>Review &amp; Submit</h2><p>Review completed sections before creating the employee record.</p></div>
                        <span class="registration-section-status neutral"><i class="far fa-clock"></i> Pending</span>
                        <i class="fas fa-chevron-down registration-chevron"></i>
                        <span class="step-pill">Step 6 of 6</span>
                    </div>
                    <div class="review-grid">
                        <div class="review-card"><div class="review-name"><span class="review-check">&#10003;</span>Basic Employee Information</div><button type="button" class="btn-modern btn-secondary-modern" data-edit-step="1">Edit</button></div>
                        <div class="review-card"><div class="review-name"><span class="review-check">&#10003;</span>Employment Information</div><button type="button" class="btn-modern btn-secondary-modern" data-edit-step="2">Edit</button></div>
                        <div class="review-card"><div class="review-name"><span class="review-check">&#10003;</span>Organization Details</div><button type="button" class="btn-modern btn-secondary-modern" data-edit-step="3">Edit</button></div>
                        <div class="review-card"><div class="review-name"><span class="review-check">&#10003;</span>Attendance &amp; Shift Information</div><button type="button" class="btn-modern btn-secondary-modern" data-edit-step="4">Edit</button></div>
                        <div class="review-card"><div class="review-name"><span class="review-check">&#10003;</span>Assets Assignment</div><button type="button" class="btn-modern btn-secondary-modern" data-edit-step="5">Edit</button></div>
                    </div>
                </section>

                <div class="wizard-actions">
                    <div class="action-left">
                        <button type="button" id="backButton" class="btn-modern btn-secondary-modern">Back</button>
                    </div>
                    <div class="action-right">
                        <button type="button" id="nextButton" class="btn-modern btn-primary-modern">Save &amp; Next</button>
                        <asp:Button ID="submitButton" runat="server" ClientIDMode="Static" CssClass="btn-modern btn-primary-modern" Text="Submit Employee" OnClick="SubmitEmployee_Click" OnClientClick="return validateEmployeeRegistration();" />
                    </div>
                </div>
            </main>
        </div>

        <section id="successScreen" class="success-card">
            <div class="success-illustration"><span>&#10003;</span></div>
            <h2>Employee Created Successfully</h2>
            <p class="onboarding-subtitle">The employee profile, attendance configuration, and assets have been prepared.</p>
            <div class="credential-box">
                <div class="credential-item"><div class="credential-label">Employee ID</div><div class="credential-value"><asp:Literal ID="litEmployeeId" runat="server" /></div></div>
                <div class="credential-item"><div class="credential-label">Username</div><div class="credential-value"><asp:Literal ID="litUsername" runat="server" /></div></div>
                <div class="credential-item"><div class="credential-label">Auto Generated Password</div><div class="credential-value"><asp:Literal ID="litPassword" runat="server" /></div></div>
            </div>
            <label class="send-mail"><input id="chkSendCredentials" runat="server" type="checkbox" checked /> Send Credentials To Employee Email</label>
            <div class="action-right" style="justify-content:center;">
                <button type="button" class="btn-modern btn-secondary-modern" onclick="window.location.href='/View/Modules/EmployeeList.aspx'">Go To Employee List</button>
                <button type="button" class="btn-modern btn-primary-modern" id="createAnotherButton">Create Another Employee</button>
            </div>
        </section>
    </div>

    <script>
        function getEmployeeRegistrationRequiredFields() {
            return [
                { id: '<%= txtEmployeeCode.ClientID %>', name: 'Employee Code' },
                { id: '<%= txtUsername.ClientID %>', name: 'Username' },
                { id: '<%= txtFirstName.ClientID %>', name: 'First Name' },
                { id: '<%= txtLastName.ClientID %>', name: 'Last Name' },
                { id: '<%= txtEmail.ClientID %>', name: 'Email ID' },
                { id: '<%= txtMobileNumber.ClientID %>', name: 'Mobile Number' },
                { id: '<%= ddlGender.ClientID %>', name: 'Gender' },
                { id: '<%= txtDateOfBirth.ClientID %>', name: 'Date of Birth' },
                { id: '<%= ddlNationality.ClientID %>', name: 'Nationality' },
                { id: '<%= ddlMaritalStatus.ClientID %>', name: 'Marital Status' },
                { id: '<%= ddlBloodGroup.ClientID %>', name: 'Blood Group' },
                { id: '<%= ddlEmploymentType.ClientID %>', name: 'Employment Type' },
                { id: '<%= ddlEmployeeCategory.ClientID %>', name: 'Employee Category' },
                { id: '<%= txtJoiningDate.ClientID %>', name: 'Joining Date' },
                { id: '<%= ddlProbationPeriod.ClientID %>', name: 'Probation Period' },
                { id: '<%= ddlEmployeeStatus.ClientID %>', name: 'Employee Status' },
                { id: '<%= ddlCompany.ClientID %>', name: 'Company' },
                { id: '<%= ddlDepartment.ClientID %>', name: 'Department' },
                { id: '<%= txtBranchOffice.ClientID %>', name: 'Branch Office' },
                { id: '<%= txtLocation.ClientID %>', name: 'Location' },
                { id: '<%= ddlDesignation.ClientID %>', name: 'Designation' },
                { id: '<%= ddlReportingManager.ClientID %>', name: 'Reporting Manager' },
                { id: '<%= ddlFunctionalManager.ClientID %>', name: 'Functional Manager' },
                { id: '<%= ddlHod.ClientID %>', name: 'HOD' },
                { id: '<%= ddlAttendanceType.ClientID %>', name: 'Attendance Type' },
                { id: '<%= ddlWeeklyOff.ClientID %>', name: 'Weekly Off' },
                { id: '<%= txtWorkingHours.ClientID %>', name: 'Working Hours' },
                { id: '<%= ddlAttendancePolicy.ClientID %>', name: 'Attendance Policy' },
                { id: '<%= txtPunchingDeviceId.ClientID %>', name: 'Punching Device ID' },
                { id: '<%= txtBiometricId.ClientID %>', name: 'Biometric ID' },
                { id: '<%= ddlWorkLocation.ClientID %>', name: 'Work Location' },
                { id: '<%= txtAssetType.ClientID %>', name: 'Asset Type' },
                { id: '<%= txtAssetNumber.ClientID %>', name: 'Asset Number' },
                { id: '<%= txtAssetName.ClientID %>', name: 'Asset Name' },
                { id: '<%= txtAssignedDate.ClientID %>', name: 'Assigned Date' },
                { id: '<%= ddlAssetCondition.ClientID %>', name: 'Asset Condition' },
                { id: '<%= ddlAssetStatus.ClientID %>', name: 'Asset Status' }
            ];
        }

        function showMandatoryFieldMessage(fieldName) {
            if (window.Swal) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Please fill mandatory fields',
                    text: fieldName + ' is required.',
                    confirmButtonColor: '#2563EB'
                });
            } else {
                alert(fieldName + ' is required.');
            }
        }

        function isOvertimeEligibleChecked() {
            var overtimeToggle = document.getElementById('<%= chkOvertimeEligible.ClientID %>');
            return !!(overtimeToggle && overtimeToggle.checked);
        }

        function validateEmployeeFields(section) {
            var requiredFields = getEmployeeRegistrationRequiredFields();
            if (isOvertimeEligibleChecked()) {
                requiredFields.push({ id: '<%= txtOvertimeRate.ClientID %>', name: 'Overtime Rate' });
            }
            var missing = [];
            var firstInvalid = null;

            requiredFields.forEach(function (field) {
                var element = document.getElementById(field.id);
                if (!element) {
                    return;
                }

                if (section && element.closest('.wizard-step') !== section) {
                    return;
                }

                var value = (element.value || '').trim();
                var isMissing = value === '';
                element.classList.toggle('validation-error', isMissing);

                if (isMissing) {
                    missing.push(field.name);
                    if (!firstInvalid) {
                        firstInvalid = element;
                    }
                }
            });

            if (!firstInvalid) {
                return validateEmployeeInlineRules(section);
            }

            var section = firstInvalid.closest('.wizard-step');
            if (section) {
                document.querySelectorAll('.wizard-step').forEach(function (panel) {
                    panel.classList.toggle('collapsed', panel !== section);
                });
            }

            firstInvalid.focus();

            showMandatoryFieldMessage(missing[0]);

            return false;
        }

        function validateEmployeeInlineRules(section) {
            var fieldsToCheck = [
                { field: 'FirstName', id: '<%= txtFirstName.ClientID %>' },
                { field: 'MiddleName', id: '<%= txtMiddleName.ClientID %>' },
                { field: 'LastName', id: '<%= txtLastName.ClientID %>' },
                { field: 'Email', id: '<%= txtEmail.ClientID %>' },
                { field: 'Mobile', id: '<%= txtMobileNumber.ClientID %>' },
                { field: 'WorkingHours', id: '<%= txtWorkingHours.ClientID %>' }
            ];

            for (var i = 0; i < fieldsToCheck.length; i++) {
                var item = fieldsToCheck[i];
                var element = document.getElementById(item.id);
                if (!element || (section && element.closest('.wizard-step') !== section)) {
                    continue;
                }

                var message = getEmployeeFormatMessage(item.field, element.value.trim());
                employeeFormatState[item.field] = !!message;
                setEmployeeValidationMessage(item.field, element, message);

                if (message) {
                    element.focus();
                    return false;
                }
            }

            return validateEmployeeAssets(section);
        }

        function validateEmployeeRegistration() {
            return validateEmployeeFields() && validateEmployeeDuplicatesBeforeSubmit();
        }

        function validateEmployeeAssets(section) {
            var assetSection = document.querySelector('.wizard-step[data-step="5"]');
            if (section && section !== assetSection) {
                return true;
            }

            var hiddenAssets = document.getElementById('hdnAssetsJson');
            var rows = document.querySelectorAll('#assetRows tr.asset-row');
            var assets = [];
            var firstInvalid = null;
            var missingName = '';

            rows.forEach(function (row, index) {
                var inputs = row.querySelectorAll('input');
                var selects = row.querySelectorAll('select');
                var asset = {
                    asset_type: inputs[0] ? inputs[0].value.trim() : '',
                    asset_number: inputs[1] ? inputs[1].value.trim() : '',
                    asset_name: inputs[2] ? inputs[2].value.trim() : '',
                    assigned_date: inputs[3] ? inputs[3].value.trim() : '',
                    return_date: inputs[4] ? inputs[4].value.trim() : '',
                    asset_condition: selects[0] ? selects[0].value : '',
                    asset_status: selects[1] ? selects[1].value : ''
                };

                [
                    { element: inputs[0], value: asset.asset_type, name: 'Asset Type' },
                    { element: inputs[1], value: asset.asset_number, name: 'Asset Number' },
                    { element: inputs[2], value: asset.asset_name, name: 'Asset Name' },
                    { element: inputs[3], value: asset.assigned_date, name: 'Assigned Date' },
                    { element: selects[0], value: asset.asset_condition, name: 'Asset Condition' },
                    { element: selects[1], value: asset.asset_status, name: 'Asset Status' }
                ].forEach(function (field) {
                    var isMissing = !field.value;
                    if (field.element) {
                        field.element.classList.toggle('validation-error', isMissing);
                    }

                    if (isMissing && !firstInvalid) {
                        firstInvalid = field.element;
                        missingName = rows.length > 1 ? field.name + ' is required for asset row ' + (index + 1) : field.name;
                    }
                });

                assets.push(asset);
            });

            if (firstInvalid) {
                if (assetSection) {
                    document.querySelectorAll('.wizard-step').forEach(function (panel) {
                        panel.classList.toggle('collapsed', panel !== assetSection);
                    });
                }
                firstInvalid.focus();
                showMandatoryFieldMessage(missingName);
                return false;
            }

            if (hiddenAssets) {
                hiddenAssets.value = JSON.stringify(assets);
            }

            return true;
        }

        var employeeDuplicateState = {};
        var employeeDuplicateLastShown = {};
        var employeeFormatState = {};
        var employeeDuplicateCheckUrl = '<%= ResolveUrl("~/View/Modules/EmployeeRegistration.aspx") %>';

        function setEmployeeValidationMessage(fieldName, element, message) {
            var messageMap = {
                EmployeeCode: 'employeeCodeDuplicateMessage',
                Username: 'usernameDuplicateMessage',
                FirstName: 'firstNameValidationMessage',
                MiddleName: 'middleNameValidationMessage',
                LastName: 'lastNameValidationMessage',
                Email: 'emailDuplicateMessage',
                Mobile: 'mobileDuplicateMessage',
                WorkingHours: 'workingHoursValidationMessage'
            };
            var messageElement = document.getElementById(messageMap[fieldName]);

            if (element) {
                element.classList.toggle('validation-error', !!message);
            }

            if (messageElement) {
                messageElement.textContent = message || '';
                messageElement.style.display = message ? 'block' : '';
            }
        }

        function checkEmployeeDuplicateField(fieldName, element) {
            if (!element || !element.value || !element.value.trim()) {
                employeeDuplicateState[fieldName] = false;
                if (element) {
                    setEmployeeValidationMessage(fieldName, element, '');
                }
                return;
            }

            var formatMessage = getEmployeeFormatMessage(fieldName, element.value.trim());
            if (formatMessage) {
                employeeDuplicateState[fieldName] = false;
                employeeFormatState[fieldName] = true;
                setEmployeeValidationMessage(fieldName, element, formatMessage);
                return;
            }

            fetch(employeeDuplicateCheckUrl
                + '?action=checkDuplicate'
                + '&fieldName=' + encodeURIComponent(fieldName)
                + '&fieldValue=' + encodeURIComponent(element.value.trim()), {
                method: 'GET',
                credentials: 'same-origin',
                headers: {
                    'Accept': 'application/json'
                }
            })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('Duplicate validation failed.');
                    }
                    return response.json();
                })
                .then(function (response) {
                    response = response && response.d ? response.d : response;
                    var isDuplicate = response && response.IsDuplicate;
                    employeeDuplicateState[fieldName] = !!isDuplicate;
                    employeeFormatState[fieldName] = false;
                    employeeDuplicateLastShown[fieldName] = isDuplicate ? element.value.trim() : '';
                    setEmployeeValidationMessage(fieldName, element, isDuplicate && response.Message ? response.Message : '');
                })
                .catch(function () {
                    employeeDuplicateState[fieldName] = false;
                });
        }

        function getEmployeeFormatMessage(fieldName, value) {
            if (!value) {
                return '';
            }

            if (fieldName === 'Email') {
                return /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(value)
                    ? ''
                    : 'Enter a valid Email ID.';
            }

            if (fieldName === 'FirstName' || fieldName === 'MiddleName' || fieldName === 'LastName') {
                return /^[A-Za-z ]+$/.test(value)
                    ? ''
                    : 'Write name in correct format.';
            }

            if (fieldName === 'Mobile') {
                return /^\d{10}$/.test(value)
                    ? ''
                    : 'Mobile Number must be 10 digits.';
            }

            if (fieldName === 'WorkingHours') {
                return /^([0-9]|[01][0-9]|2[0-3]):[0-5][0-9]$/.test(value)
                    ? ''
                    : 'Working Hours must be in HH:mm format.';
            }

            return '';
        }

        function validateEmployeeDuplicatesBeforeSubmit() {
            var duplicateMessage = '';
            if (employeeFormatState.Email) {
                duplicateMessage = 'Enter a valid Email ID.';
            } else if (employeeFormatState.Mobile) {
                duplicateMessage = 'Mobile Number must be 10 digits.';
            } else if (employeeFormatState.FirstName || employeeFormatState.MiddleName || employeeFormatState.LastName) {
                duplicateMessage = 'Write name in correct format.';
            } else if (employeeFormatState.WorkingHours) {
                duplicateMessage = 'Working Hours must be in HH:mm format.';
            } else if (employeeDuplicateState.EmployeeCode) {
                duplicateMessage = 'Employee Code already exists.';
            } else if (employeeDuplicateState.Username) {
                duplicateMessage = 'Username already exists.';
            } else if (employeeDuplicateState.Email) {
                duplicateMessage = 'Email ID already exists.';
            } else if (employeeDuplicateState.Mobile) {
                duplicateMessage = 'Mobile Number already exists.';
            }

            if (!duplicateMessage) {
                return true;
            }

            var duplicateElement = employeeDuplicateState.EmployeeCode
                ? document.getElementById('<%= txtEmployeeCode.ClientID %>')
                : employeeDuplicateState.Username
                    ? document.getElementById('<%= txtUsername.ClientID %>')
                    : (employeeDuplicateState.Email || employeeFormatState.Email)
                        ? document.getElementById('<%= txtEmail.ClientID %>')
                        : document.getElementById('<%= txtMobileNumber.ClientID %>');
            if (duplicateElement) {
                duplicateElement.focus();
            }
            return false;
        }

        (function () {
            var wizardArea = document.getElementById('wizardArea');
            var successScreen = document.getElementById('successScreen');
            var statusText = document.getElementById('statusText');
            var backButton = document.getElementById('backButton');
            var createAnotherButton = document.getElementById('createAnotherButton');
            var assignAssetButton = document.getElementById('assignAssetButton');
            var assetRows = document.getElementById('assetRows');
            var overtimeEligible = document.getElementById('<%= chkOvertimeEligible.ClientID %>');
            var overtimeRate = document.getElementById('<%= txtOvertimeRate.ClientID %>');
            var overtimeRateLabel = document.getElementById('overtimeRateLabel');
            var employeeCode = document.getElementById('<%= txtEmployeeCode.ClientID %>');
            var employeeUsername = document.getElementById('<%= txtUsername.ClientID %>');
            var firstName = document.getElementById('<%= txtFirstName.ClientID %>');
            var middleName = document.getElementById('<%= txtMiddleName.ClientID %>');
            var lastName = document.getElementById('<%= txtLastName.ClientID %>');
            var employeeEmail = document.getElementById('<%= txtEmail.ClientID %>');
            var employeeMobile = document.getElementById('<%= txtMobileNumber.ClientID %>');
            var workingHours = document.getElementById('<%= txtWorkingHours.ClientID %>');

            function setSectionStatus(section, status) {
                var statusElement = section ? section.querySelector('.registration-section-status') : null;
                if (!statusElement) {
                    return;
                }

                statusElement.classList.remove('saved', 'neutral');
                if (status === 'saved') {
                    statusElement.classList.add('saved');
                    statusElement.innerHTML = '<i class="fas fa-check-circle"></i> Saved';
                } else if (status === 'neutral') {
                    statusElement.classList.add('neutral');
                    statusElement.innerHTML = '<i class="far fa-clock"></i> Pending';
                } else {
                    statusElement.innerHTML = '<i class="far fa-clock"></i> Pending';
                }
            }

            function openSection(section) {
                if (!section) {
                    return;
                }

                document.querySelectorAll('.wizard-step').forEach(function (panel) {
                    panel.classList.toggle('collapsed', panel !== section);
                });

                section.classList.remove('pending-neutral');
                statusText.textContent = section.classList.contains('pending') ? 'Pending sections' : 'Draft in progress';
            }

            function toggleSection(section) {
                if (!section) {
                    return;
                }

                if (!section.classList.contains('collapsed')) {
                    section.classList.add('collapsed');
                    return;
                }

                openSection(section);
            }

            document.querySelectorAll('.wizard-step .card-heading').forEach(function (heading) {
                heading.addEventListener('click', function () {
                    toggleSection(heading.closest('.wizard-step'));
                });
            });

            document.querySelectorAll('[data-edit-step]').forEach(function (button) {
                button.addEventListener('click', function () {
                    openSection(document.querySelector('.wizard-step[data-step="' + button.getAttribute('data-edit-step') + '"]'));
                });
            });

            document.querySelectorAll('[data-collapse-step]').forEach(function (button) {
                button.addEventListener('click', function () {
                    var current = button.closest('.wizard-step');
                    var next = current ? current.nextElementSibling : null;
                    if (current) {
                        if (!validateEmployeeFields(current)) {
                            current.classList.add('pending');
                            setSectionStatus(current, 'pending');
                            return;
                        }

                        current.classList.remove('pending', 'pending-neutral');
                        setSectionStatus(current, 'saved');
                    }
                    openSection(next || current);
                });
            });

            function syncOvertimeRateState() {
                var isEligible = !!(overtimeEligible && overtimeEligible.checked);
                if (overtimeRate) {
                    overtimeRate.disabled = !isEligible;
                    if (!isEligible) {
                        overtimeRate.value = '';
                        overtimeRate.classList.remove('validation-error');
                    }
                }
                if (overtimeRateLabel) {
                    overtimeRateLabel.classList.toggle('is-required', isEligible);
                }
            }

            if (overtimeEligible) {
                overtimeEligible.addEventListener('change', syncOvertimeRateState);
                syncOvertimeRateState();
            }

            [
                { field: 'EmployeeCode', element: employeeCode },
                { field: 'Username', element: employeeUsername },
                { field: 'Email', element: employeeEmail },
                { field: 'Mobile', element: employeeMobile }
            ].forEach(function (item) {
                if (!item.element) {
                    return;
                }

                var handleFieldInput = function () {
                    if (item.field === 'Mobile') {
                        var digitsOnly = item.element.value.replace(/\D/g, '').slice(0, 10);
                        if (item.element.value !== digitsOnly) {
                            item.element.value = digitsOnly;
                        }
                    }

                    employeeDuplicateState[item.field] = false;
                    employeeDuplicateLastShown[item.field] = '';

                    var formatMessage = getEmployeeFormatMessage(item.field, item.element.value.trim());
                    employeeFormatState[item.field] = !!formatMessage;
                    setEmployeeValidationMessage(item.field, item.element, formatMessage);
                };

                if (item.field === 'Mobile') {
                    item.element.addEventListener('keydown', function (event) {
                        var allowedKeys = ['Backspace', 'Delete', 'Tab', 'ArrowLeft', 'ArrowRight', 'Home', 'End'];
                        if (allowedKeys.indexOf(event.key) >= 0 || event.ctrlKey || event.metaKey) {
                            return;
                        }

                        if (!/^\d$/.test(event.key) || item.element.value.length >= 10) {
                            event.preventDefault();
                        }
                    });

                    item.element.addEventListener('paste', function (event) {
                        event.preventDefault();
                        var pastedText = (event.clipboardData || window.clipboardData).getData('text') || '';
                        item.element.value = (item.element.value + pastedText).replace(/\D/g, '').slice(0, 10);
                        handleFieldInput();
                    });
                }

                item.element.addEventListener('blur', function () {
                    checkEmployeeDuplicateField(item.field, item.element);
                });

                item.element.addEventListener('input', function () {
                    handleFieldInput();
                });
            });

            [
                { field: 'FirstName', element: firstName },
                { field: 'MiddleName', element: middleName },
                { field: 'LastName', element: lastName }
            ].forEach(function (item) {
                if (!item.element) {
                    return;
                }

                var syncNameValue = function () {
                    var cleanValue = item.element.value.replace(/[^A-Za-z ]/g, '');
                    if (item.element.value !== cleanValue) {
                        item.element.value = cleanValue;
                    }

                    var message = getEmployeeFormatMessage(item.field, item.element.value.trim());
                    employeeFormatState[item.field] = !!message;
                    setEmployeeValidationMessage(item.field, item.element, message);
                };

                item.element.addEventListener('keydown', function (event) {
                    var allowedKeys = ['Backspace', 'Delete', 'Tab', 'ArrowLeft', 'ArrowRight', 'Home', 'End'];
                    if (allowedKeys.indexOf(event.key) >= 0 || event.ctrlKey || event.metaKey) {
                        return;
                    }

                    if (!/^[A-Za-z ]$/.test(event.key)) {
                        event.preventDefault();
                    }
                });

                item.element.addEventListener('paste', function (event) {
                    event.preventDefault();
                    var pastedText = (event.clipboardData || window.clipboardData).getData('text') || '';
                    item.element.value = item.element.value + pastedText.replace(/[^A-Za-z ]/g, '');
                    syncNameValue();
                });

                item.element.addEventListener('input', syncNameValue);
            });

            if (workingHours) {
                var syncWorkingHoursValue = function () {
                    var value = workingHours.value.replace(/[^0-9:]/g, '');
                    var parts = value.split(':');
                    if (parts.length > 2) {
                        value = parts[0] + ':' + parts.slice(1).join('');
                    }

                    if (value.indexOf(':') >= 0) {
                        var timeParts = value.split(':');
                        value = timeParts[0].slice(0, 2) + ':' + timeParts[1].slice(0, 2);
                    } else if (value.length > 2) {
                        value = value.slice(0, 2) + ':' + value.slice(2, 4);
                    }

                    if (workingHours.value !== value) {
                        workingHours.value = value;
                    }

                    var message = getEmployeeFormatMessage('WorkingHours', workingHours.value.trim());
                    employeeFormatState.WorkingHours = !!message;
                    setEmployeeValidationMessage('WorkingHours', workingHours, message);
                };

                workingHours.addEventListener('keydown', function (event) {
                    var allowedKeys = ['Backspace', 'Delete', 'Tab', 'ArrowLeft', 'ArrowRight', 'Home', 'End'];
                    if (allowedKeys.indexOf(event.key) >= 0 || event.ctrlKey || event.metaKey) {
                        return;
                    }

                    if (!/^[0-9:]$/.test(event.key) || (event.key === ':' && workingHours.value.indexOf(':') >= 0)) {
                        event.preventDefault();
                    }
                });

                workingHours.addEventListener('paste', function (event) {
                    event.preventDefault();
                    var pastedText = (event.clipboardData || window.clipboardData).getData('text') || '';
                    workingHours.value = workingHours.value + pastedText;
                    syncWorkingHoursValue();
                });

                workingHours.addEventListener('input', syncWorkingHoursValue);
            }

            function closeMobileSelectMenus(exceptWrap) {
                document.querySelectorAll('.mobile-select-wrap.open').forEach(function (wrap) {
                    if (wrap !== exceptWrap) {
                        wrap.classList.remove('open');
                    }
                });
            }

            function updateMobileSelect(select) {
                if (!select) {
                    return;
                }

                var wrap = select.closest('.mobile-select-wrap');
                if (!wrap) {
                    return;
                }

                var trigger = wrap.querySelector('.mobile-select-trigger');
                var menu = wrap.querySelector('.mobile-select-menu');
                var selected = select.options[select.selectedIndex];

                if (trigger) {
                    trigger.textContent = selected ? selected.text : '-- Select --';
                    trigger.disabled = select.disabled;
                }

                if (menu) {
                    menu.querySelectorAll('.mobile-select-option').forEach(function (item) {
                        item.classList.toggle('selected', item.getAttribute('data-value') === select.value);
                    });
                }
            }

            function sizeMobileSelectMenu(wrap) {
                if (!wrap) {
                    return;
                }

                var trigger = wrap.querySelector('.mobile-select-trigger');
                var menu = wrap.querySelector('.mobile-select-menu');
                if (!trigger || !menu) {
                    return;
                }

                var width = trigger.getBoundingClientRect().width;
                menu.style.width = width + 'px';
                menu.style.minWidth = width + 'px';
                menu.style.maxWidth = width + 'px';
            }

            function enhanceMobileSelect(select) {
                if (!select || select.getAttribute('data-mobile-select-ready') === 'true') {
                    return;
                }

                select.setAttribute('data-mobile-select-ready', 'true');
                select.classList.add('mobile-select-native');

                var wrap = document.createElement('div');
                wrap.className = 'mobile-select-wrap';
                select.parentNode.insertBefore(wrap, select);
                wrap.appendChild(select);

                var trigger = document.createElement('button');
                trigger.type = 'button';
                trigger.className = 'mobile-select-trigger';

                var menu = document.createElement('ul');
                menu.className = 'mobile-select-menu';

                Array.prototype.slice.call(select.options).forEach(function (option, index) {
                    var isPlaceholder = option.value === '' && index === 0;
                    if (isPlaceholder) {
                        return;
                    }

                    var item = document.createElement('li');
                    item.className = 'mobile-select-option';
                    item.textContent = option.text;
                    item.setAttribute('data-value', option.value);

                    if (option.disabled) {
                        item.classList.add('disabled');
                    }

                    item.addEventListener('click', function () {
                        if (option.disabled || select.disabled) {
                            return;
                        }

                        select.value = option.value;
                        select.dispatchEvent(new Event('change', { bubbles: true }));
                        select.dispatchEvent(new Event('input', { bubbles: true }));
                        updateMobileSelect(select);
                        wrap.classList.remove('open');
                    });

                    menu.appendChild(item);
                });

                trigger.addEventListener('click', function (event) {
                    event.preventDefault();
                    event.stopPropagation();
                    if (select.disabled) {
                        return;
                    }

                    var shouldOpen = !wrap.classList.contains('open');
                    closeMobileSelectMenus(wrap);
                    wrap.classList.toggle('open', shouldOpen);
                    if (shouldOpen) {
                        sizeMobileSelectMenu(wrap);
                    }
                });

                select.addEventListener('change', function () {
                    updateMobileSelect(select);
                });

                wrap.appendChild(trigger);
                wrap.appendChild(menu);
                updateMobileSelect(select);
                sizeMobileSelectMenu(wrap);
            }

            function initMobileSelects(scope) {
                (scope || document).querySelectorAll('select.form-select-modern').forEach(enhanceMobileSelect);
            }

            function bindRealtimeRequiredFieldCleanup() {
                getEmployeeRegistrationRequiredFields().forEach(function (field) {
                    var element = document.getElementById(field.id);
                    if (!element) {
                        return;
                    }

                    var clearValidation = function () {
                        var value = (element.value || '').trim();
                        if (value !== '') {
                            element.classList.remove('validation-error');
                        }
                    };

                    element.addEventListener('input', clearValidation);
                    element.addEventListener('change', clearValidation);
                    element.addEventListener('blur', clearValidation);
                });

                document.querySelectorAll('input[type="date"]').forEach(function (element) {
                    var clearDateValidation = function () {
                        if ((element.value || '').trim() !== '') {
                            element.classList.remove('validation-error');
                        }
                    };

                    element.addEventListener('change', clearDateValidation);
                    element.addEventListener('blur', clearDateValidation);
                });
            }

            document.addEventListener('click', function () {
                closeMobileSelectMenus();
            });

            window.addEventListener('resize', function () {
                document.querySelectorAll('.mobile-select-wrap').forEach(sizeMobileSelectMenu);
            });

            initMobileSelects(document);
            bindRealtimeRequiredFieldCleanup();

            function clearAssetRow(row) {
                row.querySelectorAll('input').forEach(function (input) {
                    input.value = '';
                });
                row.querySelectorAll('select').forEach(function (select) {
                    select.selectedIndex = 0;
                    updateMobileSelect(select);
                });
            }

            function bindAssetDelete(button) {
                button.addEventListener('click', function () {
                    var row = button.closest('tr');
                    if (!row || !assetRows) {
                        return;
                    }

                    row.remove();
                });
            }

            function applyNoFutureDates(scope) {
                var root = scope || document;
                var today = new Date().toISOString().split('T')[0];
                root.querySelectorAll('input[type="date"]').forEach(function (input) {
                    input.setAttribute('max', today);
                });
            }

            function createAssetRow() {
                var conditionSelect = document.getElementById('<%= ddlAssetCondition.ClientID %>');
                var statusSelect = document.getElementById('<%= ddlAssetStatus.ClientID %>');
                var row = document.createElement('tr');
                row.className = 'asset-row';
                row.innerHTML =
                    '<td><input class="form-control-modern" placeholder="Enter asset type" /></td>' +
                    '<td><input class="form-control-modern" /></td>' +
                    '<td><input class="form-control-modern" /></td>' +
                    '<td><input class="form-control-modern" type="date" /></td>' +
                    '<td><input class="form-control-modern" type="date" /></td>' +
                    '<td><select class="form-select-modern">' + (conditionSelect ? conditionSelect.innerHTML : '') + '</select></td>' +
                    '<td><select class="form-select-modern">' + (statusSelect ? statusSelect.innerHTML : '') + '</select></td>' +
                    '<td><button type="button" class="asset-delete-btn" data-delete-asset title="Delete asset"><i class="fas fa-trash-alt"></i></button></td>';
                return row;
            }

            if (assignAssetButton && assetRows) {
                assignAssetButton.addEventListener('click', function () {
                    var row = createAssetRow();
                    assetRows.appendChild(row);
                    initMobileSelects(row);
                    bindAssetDelete(row.querySelector('[data-delete-asset]'));
                    applyNoFutureDates(row);
                });
            }

            applyNoFutureDates(document);

            if (backButton) {
                backButton.addEventListener('click', function () {
                    var current = document.querySelector('.wizard-step:not(.collapsed)');
                    var previous = current ? current.previousElementSibling : null;

                    while (previous && !previous.classList.contains('wizard-step')) {
                        previous = previous.previousElementSibling;
                    }

                    if (previous) {
                        openSection(previous);
                        return;
                    }

                    window.location.href = '/View/Modules/Home.aspx';
                });
            }

            if (createAnotherButton) {
                createAnotherButton.addEventListener('click', function () {
                    successScreen.classList.remove('active');
                    wizardArea.style.display = 'block';
                    openSection(document.querySelector('.wizard-step[data-step="1"]'));
                });
            }
        })();
    </script>
</asp:Content>
