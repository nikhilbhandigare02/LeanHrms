<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="AddEmployee.aspx.cs" Inherits="HRMS.View.Modules.AddEmployee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../../assets/libs/jquery/jquery.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <style>
        :root {
            --hrms-primary: #2563eb;
            --hrms-border: #dbe4f0;
            --hrms-text: #10213f;
            --hrms-muted: #64748b;
            --hrms-bg: #f6f9fe;
            --hrms-success: #16a34a;
            --hrms-warning: #f97316;
        }

        .employee-update-page {
            max-width: 1380px;
            margin: 0 auto;
            padding: 8px 18px 28px;
            color: var(--hrms-text);
        }

        .employee-breadcrumb {
            display: flex;
            align-items: center;
            gap: 10px;
            color: #31537f;
            font-size: 12px;
            font-weight: 700;
            margin-bottom: 12px;
        }

        .employee-breadcrumb span:last-child {
            color: var(--hrms-muted);
            font-weight: 600;
        }

        .employee-title-row {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 16px;
            margin-bottom: 14px;
        }

        .employee-update-title {
            display: block;
            font-size: 22px;
            line-height: 1.2;
            font-weight: 800;
            color: #071733;
            margin-bottom: 4px;
        }

        .employee-update-subtitle {
            font-size: 13px;
            color: #425b7c;
            margin: 0;
        }

        .employee-profile-strip {
            background: #fff;
            border: 1px solid var(--hrms-border);
            border-radius: 8px;
            box-shadow: 0 12px 30px rgba(15, 23, 42, 0.05);
            padding: 20px 24px;
            display: grid;
            grid-template-columns: minmax(300px, 1.45fr) minmax(230px, 1fr) minmax(140px, .72fr) minmax(150px, .78fr) minmax(170px, .82fr);
            gap: 0;
            align-items: center;
            margin-bottom: 14px;
        }

        .profile-identity {
            display: flex;
            align-items: center;
            gap: 14px;
            min-width: 0;
        }

        .profile-avatar {
            width: 62px;
            height: 62px;
            border-radius: 50%;
            background: #eaf1ff;
            color: #1e3a8a;
            display: grid;
            place-items: center;
            flex: 0 0 auto;
        }

        .profile-avatar i {
            font-size: 28px;
        }

        .profile-code {
            font-weight: 800;
            color: #14315d;
            font-size: 13px;
        }

        .profile-name {
            font-weight: 800;
            color: #071733;
            font-size: 15px;
            margin-top: 4px;
        }

        .profile-meta {
            font-size: 12px;
            color: #526987;
            margin-top: 3px;
        }

        .profile-active {
            display: inline-flex;
            align-items: center;
            padding: 3px 9px;
            border-radius: 999px;
            background: #dcfce7;
            color: #15803d;
            font-size: 11px;
            font-weight: 800;
            margin-left: 8px;
        }

        .profile-stat {
            border-left: 1px solid #e6edf6;
            min-height: 52px;
            min-width: 0;
            padding: 0 22px;
            display: flex;
            flex-direction: column;
            justify-content: center;
        }

        .profile-stat-label {
            font-size: 11px;
            font-weight: 800;
            color: #6b7d99;
            margin-bottom: 6px;
        }

        .profile-stat-value {
            font-size: 13px;
            font-weight: 700;
            color: #1d3151;
            line-height: 1.35;
            overflow-wrap: anywhere;
            word-break: normal;
        }

        .view-profile-button {
            border: 1px solid var(--hrms-primary);
            background: #fff;
            color: var(--hrms-primary);
            border-radius: 5px;
            min-height: 38px;
            padding: 0 16px;
            font-size: 12px;
            font-weight: 800;
        }

        .update-accordion-stack {
            display: grid;
            gap: 8px;
        }

        .update-section {
            background: #fff;
            border: 1px solid var(--hrms-border);
            border-radius: 8px;
            box-shadow: 0 8px 22px rgba(15, 23, 42, 0.035);
            overflow: hidden;
        }

        .update-section summary {
            min-height: 54px;
            padding: 0 18px;
            display: flex;
            align-items: center;
            gap: 12px;
            cursor: pointer;
            list-style: none;
            user-select: none;
        }

        .update-section summary::-webkit-details-marker {
            display: none;
        }

        .section-icon {
            width: 30px;
            height: 30px;
            border-radius: 50%;
            background: #edf4ff;
            color: var(--hrms-primary);
            display: grid;
            place-items: center;
            flex: 0 0 auto;
            font-size: 14px;
        }

        .section-title {
            font-size: 13px;
            font-weight: 800;
            color: #10213f;
            flex: 1;
        }

        .section-owner {
            font-size: 11px;
            font-weight: 800;
            color: #6b7d99;
            background: #f4f7fb;
            border-radius: 999px;
            padding: 4px 10px;
        }

        .section-status {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-size: 12px;
            font-weight: 800;
            min-width: 88px;
            justify-content: flex-end;
        }

        .section-status.completed {
            color: var(--hrms-success);
        }

        .section-status.pending {
            color: var(--hrms-warning);
        }

        .section-chevron {
            color: #1e3a8a;
            transition: transform .18s ease;
        }

        .update-section[open] .section-chevron {
            transform: rotate(180deg);
        }

        .section-body {
            border-top: 1px solid #edf2f7;
            padding: 18px;
            background: #fff;
        }

        .form-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 16px 18px;
        }

        .form-grid.three {
            grid-template-columns: repeat(3, minmax(0, 1fr));
        }

        .field-block label {
            display: block;
            color: #213855;
            font-size: 12px;
            font-weight: 800;
            margin-bottom: 7px;
        }

        .field-block label.required-label::after {
            content: " *";
            color: #ef4444;
            font-weight: 800;
        }

        .field-block .form-control,
        .field-block .custom-dropdown,
        .field-block select,
        .field-block input {
            width: 100%;
            border: 1px solid #cfdbea;
            border-radius: 5px;
            min-height: 38px;
            color: #10213f;
            font-size: 13px;
            box-shadow: none;
        }

        .field-block .form-control:focus,
        .field-block select:focus,
        .field-block input:focus {
            border-color: var(--hrms-primary);
            box-shadow: 0 0 0 3px rgba(37, 99, 235, .12);
        }

        .field-block .validation-error,
        .field-block .form-control.validation-error,
        .field-block select.validation-error,
        .field-block input.validation-error {
            border-color: #ef4444 !important;
            box-shadow: 0 0 0 3px rgba(239, 68, 68, .14) !important;
        }

        .inline-field-message {
            color: #ef4444;
            display: none;
            font-size: 12px;
            font-weight: 700;
            margin-top: 6px;
        }


        .readonly-note {
            color: #66758d;
            font-size: 12px;
            padding: 12px 14px;
            background: #f8fbff;
            border: 1px dashed #cdd9ea;
            border-radius: 6px;
        }

        .contact-action-row {
            display: flex;
            justify-content: flex-end;
            margin-top: 14px;
        }

        .view-only-section .form-control[readonly],
        .view-only-section .form-control:disabled,
        .view-only-section textarea[readonly],
        .view-only-section select:disabled {
            background: #f8fbff;
            color: #425b7c;
            cursor: default;
        }

        .view-only-section input[type="checkbox"]:disabled {
            cursor: default;
        }

        .contact-view-card {
            display: none !important;
        }


        .asset-table-wrap {
            border: 1px solid #e1e8f2;
            border-radius: 8px;
            overflow-x: auto;
            background: #fff;
        }

        .entry-table-wrap {
            border: 1px solid #e1e8f2;
            border-radius: 16px;
            overflow-x: auto;
            background: #fff;
            box-shadow: 0 12px 30px rgba(15, 23, 42, 0.04);
        }

        .entry-table {
            width: 100%;
            min-width: 920px;
            border-collapse: separate;
            border-spacing: 0;
            color: #10213f;
            font-size: 13px;
        }

        .entry-table th {
            background: linear-gradient(180deg, #fbfdff 0%, #f6f9ff 100%);
            border-bottom: 1px solid #e1e8f2;
            color: #243b5a;
            font-size: 12px;
            font-weight: 800;
            padding: 16px 20px;
            text-align: left;
            white-space: nowrap;
        }

        .entry-table td {
            border-bottom: 1px solid #edf2f7;
            padding: 18px 20px;
            vertical-align: middle;
            color: #173154;
            background: #fff;
            line-height: 1.5;
        }

        .entry-table tbody tr:last-child td {
            border-bottom: 0;
        }

        .entry-table tbody tr:hover td {
            background: #fbfdff;
        }

        .entry-empty {
            color: #7a8da8;
            font-style: italic;
        }

        .entry-editor-card {
            background: #ffffff;
            border: 1px solid #dfe8f5;
            border-radius: 16px;
            margin-top: 16px;
            padding: 0;
            overflow: hidden;
            box-shadow: 0 16px 34px rgba(37, 99, 235, 0.05);
        }

        .entry-editor-card.collapsed {
            display: none;
        }

        .entry-editor-header {
            align-items: center;
            display: flex;
            justify-content: space-between;
            padding: 22px 24px;
            border-bottom: 1px solid #edf2f7;
            gap: 16px;
        }

        .entry-editor-actions {
            align-items: center;
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            margin-top: 0;
            padding: 0 24px 24px;
        }

        .entry-editor-body {
            padding: 24px;
        }

        .entry-editor-title-wrap {
            display: flex;
            align-items: center;
            gap: 14px;
            min-width: 0;
        }

        .entry-editor-badge {
            width: 46px;
            height: 46px;
            border-radius: 50%;
            background: radial-gradient(circle at top, #f1f5ff 0%, #e9f0ff 100%);
            color: #4f46e5;
            display: grid;
            place-items: center;
            flex: 0 0 auto;
            font-size: 18px;
        }

        .entry-editor-title {
            margin: 0;
            font-size: 15px;
            font-weight: 800;
            color: #10213f;
        }

        .certification-view-card {
            border: 1px solid #e3ebf6;
            border-radius: 18px;
            background: linear-gradient(180deg, #ffffff 0%, #fbfdff 100%);
            box-shadow: 0 12px 30px rgba(15, 23, 42, 0.04);
            padding: 18px 18px 20px;
        }

        .certification-view-grid {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 0 18px;
        }

        .certification-view-item {
            display: grid;
            grid-template-columns: 34px minmax(140px, 220px) 18px 1fr;
            align-items: center;
            gap: 14px;
            padding: 22px 6px;
            border-bottom: 1px solid #e9eff8;
            min-width: 0;
        }

        .certification-view-item:nth-last-child(-n+2) {
            border-bottom: 0;
        }

        .certification-view-item:nth-child(odd) {
            border-right: 1px solid #e9eff8;
            padding-right: 26px;
        }

        .certification-view-item:nth-child(even) {
            padding-left: 8px;
        }

        .certification-view-icon {
            width: 34px;
            height: 34px;
            border-radius: 10px;
            background: #f5f8ff;
            color: #657896;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
        }

        .certification-view-label {
            font-size: 12px;
            font-weight: 700;
            color: #62748f;
            line-height: 1.4;
        }

        .certification-view-colon {
            color: #8798b3;
            font-size: 18px;
            font-weight: 700;
            text-align: center;
        }

        .certification-view-value {
            font-size: 16px;
            font-weight: 700;
            color: #0f2343;
            line-height: 1.5;
            overflow-wrap: anywhere;
        }

        .certification-view-pill {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            min-height: 34px;
            padding: 0 14px;
            border-radius: 12px;
            background: #f4f7fb;
            color: #10213f;
            font-size: 14px;
            font-weight: 700;
        }

        .certification-file-panel {
            margin-top: 18px;
            border-top: 1px dashed #d9e4f4;
            padding-top: 18px;
        }

        .certification-file-card {
            border: 1px solid #e3ebf6;
            border-radius: 16px;
            background: linear-gradient(180deg, #ffffff 0%, #f9fbff 100%);
            padding: 18px 22px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 18px;
        }

        .certification-file-info {
            display: flex;
            align-items: center;
            gap: 16px;
            min-width: 0;
        }

        .certification-file-icon {
            width: 58px;
            height: 58px;
            border-radius: 14px;
            background: #f5f7ff;
            color: #ef4444;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 28px;
            flex: 0 0 auto;
        }

        .certification-file-meta {
            min-width: 0;
        }

        .certification-file-label {
            font-size: 12px;
            font-weight: 700;
            color: #6b7d99;
            margin-bottom: 6px;
        }

        .certification-file-name {
            font-size: 16px;
            font-weight: 800;
            color: #10213f;
            line-height: 1.4;
            overflow-wrap: anywhere;
        }

        .certification-file-submeta {
            margin-top: 8px;
            font-size: 12px;
            color: #6b7d99;
        }

        .certification-file-button {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
            min-width: 220px;
            min-height: 50px;
            padding: 0 20px;
            border-radius: 12px;
            border: 1.5px solid #2563eb;
            color: #2563eb;
            background: #ffffff;
            font-size: 15px;
            font-weight: 800;
            text-decoration: none;
            box-shadow: 0 10px 24px rgba(37, 99, 235, 0.06);
            transition: all .18s ease;
        }

        .certification-file-button:hover {
            background: #f5f9ff;
            color: #1d4ed8;
            text-decoration: none;
        }

        .certification-empty-file {
            font-size: 14px;
            color: #7c8ba3;
            font-weight: 600;
        }

        .certification-note {
            margin-top: 18px;
            border: 1px solid #dbe7fb;
            border-radius: 16px;
            background: linear-gradient(180deg, #fbfdff 0%, #f7faff 100%);
            padding: 18px 20px;
            display: flex;
            align-items: center;
            gap: 14px;
            color: #24406c;
        }

        .certification-note i {
            width: 34px;
            height: 34px;
            border-radius: 50%;
            background: linear-gradient(180deg, #2563eb 0%, #1d4ed8 100%);
            color: #ffffff;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
            flex: 0 0 auto;
        }

        .certification-note span {
            font-size: 14px;
            font-weight: 600;
        }

        .entry-editor-subtitle {
            margin: 4px 0 0;
            font-size: 13px;
            color: #64748b;
        }

        .switch-field {
            align-items: center;
            display: flex;
            gap: 14px;
            min-height: 38px;
        }

        .switch-label {
            color: #526987;
            font-size: 12px;
            font-weight: 800;
        }

        .yes-no-switch {
            position: relative;
            width: 56px;
            height: 32px;
            flex: 0 0 auto;
        }

        .yes-no-switch input {
            opacity: 0;
            width: 0;
            height: 0;
            position: absolute;
        }

        .yes-no-slider {
            position: absolute;
            inset: 0;
            background: #d7e3f4;
            border: 1px solid #c1d2ea;
            border-radius: 999px;
            transition: all .2s ease;
            cursor: pointer;
        }

        .yes-no-slider:before {
            content: "";
            position: absolute;
            width: 24px;
            height: 24px;
            left: 3px;
            top: 3px;
            background: #fff;
            border-radius: 50%;
            box-shadow: 0 4px 10px rgba(15, 23, 42, .18);
            transition: transform .2s ease;
        }

        .yes-no-switch input:checked + .yes-no-slider {
            background: #2563eb;
            border-color: #2563eb;
        }

        .yes-no-switch input:checked + .yes-no-slider:before {
            transform: translateX(24px);
        }

        .is-disabled {
            opacity: .65;
            pointer-events: none;
            background: #f4f7fb !important;
        }

        .asset-editor-card {
            background: #f8fbff;
            border: 1px solid #dfe8f5;
            border-radius: 8px;
            margin-top: 16px;
            padding: 16px;
            position: relative;
        }

        .asset-editor-card.collapsed {
            display: none;
        }

        .asset-editor-header {
            align-items: center;
            display: flex;
            justify-content: flex-end;
            margin-bottom: 10px;
        }

        .asset-editor-close {
            align-items: center;
            background: #fff;
            border: 1px solid #cfe0f5;
            border-radius: 8px;
            color: #64748b;
            cursor: pointer;
            display: inline-flex;
            height: 34px;
            justify-content: center;
            transition: all .18s ease;
            width: 34px;
        }

        .asset-editor-close:hover {
            background: #fee2e2;
            border-color: #fecaca;
            color: #dc2626;
            transform: translateY(-1px);
        }

        .asset-list-toolbar {
            align-items: center;
            display: flex;
            justify-content: flex-end;
            margin-bottom: 12px;
        }

        .managed-entry-block {
            display: grid;
            gap: 18px;
        }

        .managed-entry-header {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 16px;
        }

        .managed-entry-title {
            margin: 0;
            font-size: 18px;
            font-weight: 800;
            color: #11224a;
        }

        .managed-entry-subtitle {
            margin: 6px 0 0;
            font-size: 13px;
            color: #64748b;
        }

        .managed-entry-button {
            border: 0;
            border-radius: 12px;
            min-height: 54px;
            padding: 0 20px;
            display: inline-flex;
            align-items: center;
            gap: 10px;
            background: linear-gradient(135deg, #4f46e5 0%, #2563eb 100%);
            color: #fff;
            font-size: 13px;
            font-weight: 800;
            box-shadow: 0 16px 30px rgba(37, 99, 235, 0.2);
            transition: transform .18s ease, box-shadow .18s ease;
        }

        .managed-entry-button:hover {
            transform: translateY(-1px);
            box-shadow: 0 18px 34px rgba(37, 99, 235, 0.24);
        }

        .entry-chip-link {
            display: inline-flex;
            align-items: center;
            gap: 10px;
            min-height: 42px;
            padding: 8px 14px;
            border: 1px solid #d9e3f7;
            border-radius: 12px;
            background: #ffffff;
            color: #1d3151;
            text-decoration: none;
            font-weight: 700;
        }

        .entry-chip-link i {
            color: #ef4444;
            font-size: 16px;
        }

        .entry-chip-link:hover {
            background: #f8fbff;
            border-color: #b8ccf5;
            text-decoration: none;
            color: #1d3151;
        }

        .entry-chip-caption {
            display: flex;
            flex-direction: column;
            line-height: 1.25;
        }

        .entry-chip-caption small {
            color: #64748b;
            font-size: 11px;
            font-weight: 600;
        }

        .asset-editor-actions {
            align-items: center;
            display: flex;
            gap: 10px;
            justify-content: flex-end;
            margin-top: 14px;
        }

        .asset-table {
            width: 100%;
            min-width: 920px;
            border-collapse: separate;
            border-spacing: 0;
            color: #10213f;
            font-size: 13px;
        }

        .asset-table th {
            background: #f8fbff;
            border-bottom: 1px solid #e1e8f2;
            color: #243b5a;
            font-size: 12px;
            font-weight: 800;
            padding: 13px 14px;
            text-align: left;
            white-space: nowrap;
        }

        .asset-table td {
            border-bottom: 1px solid #eef3f8;
            padding: 13px 14px;
            vertical-align: middle;
        }

        .asset-table tr:last-child td {
            border-bottom: 0;
        }

        .asset-table tr:hover td {
            background: #f8fbff;
        }

        .asset-action-btn {
            align-items: center;
            background: #ffffff;
            border: 1px solid #d8e3f3;
            border-radius: 8px;
            color: #2563eb;
            display: inline-flex;
            height: 32px;
            justify-content: center;
            transition: background .18s ease, border-color .18s ease, color .18s ease, transform .18s ease;
            width: 32px;
        }

        .asset-action-btn:hover {
            background: #eff6ff;
            border-color: #afc7ff;
            color: #1d4ed8;
            transform: translateY(-1px);
        }

        .asset-action-btn.danger {
            border-color: #ffd2d7;
            color: #ef4444;
        }

        .asset-action-btn.danger:hover {
            background: #fff1f2;
            border-color: #fda4af;
            color: #dc2626;
        }

        .asset-action-stack {
            align-items: center;
            display: flex;
            gap: 8px;
        }

        .document-view-link {
            align-items: center;
            background: #ffffff;
            border: 1px solid #d8e3f3;
            border-radius: 8px;
            color: #2563eb;
            display: inline-flex;
            font-size: 13px;
            font-weight: 700;
            gap: 7px;
            height: 36px;
            justify-content: center;
            margin-top: 8px;
            padding: 0 12px;
            text-decoration: none;
            transition: background .18s ease, border-color .18s ease, color .18s ease, transform .18s ease;
        }

        .document-view-link:hover {
            background: #eff6ff;
            border-color: #afc7ff;
            color: #1d4ed8;
            text-decoration: none;
            transform: translateY(-1px);
        }

        .personal-documents-grid {
            display: grid;
            grid-template-columns: minmax(0, 1.2fr) minmax(320px, .8fr);
            gap: 18px;
            align-items: start;
        }

        .photo-card-shell {
            border: 1px solid #dfe8f5;
            border-radius: 14px;
            background: #ffffff;
            padding: 16px;
            box-shadow: 0 10px 24px rgba(15, 23, 42, 0.035);
        }

        .personal-section-layout {
            display: grid;
            grid-template-columns: minmax(0, 1fr) 360px;
            gap: 18px;
            align-items: start;
        }

        .personal-fields-stack {
            display: grid;
            gap: 14px;
        }

        .photo-card-title {
            font-size: 13px;
            font-weight: 800;
            color: #10213f;
            margin-bottom: 12px;
        }

        .employee-photo-existing {
            display: grid;
            grid-template-columns: 1fr;
            gap: 14px;
            align-items: center;
        }

        .employee-photo-existing.is-hidden,
        .employee-photo-empty.is-hidden {
            display: none;
        }

        .employee-photo-thumb {
            display: flex;
            flex-direction: column;
            gap: 10px;
            min-width: 0;
            flex: 1;
        }

        .employee-photo-file {
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .employee-photo-file-icon {
            width: 38px;
            height: 38px;
            border-radius: 10px;
            background: #eef4ff;
            color: #2563eb;
            display: grid;
            place-items: center;
            font-size: 16px;
            flex: 0 0 auto;
        }

        .employee-photo-file-name {
            font-size: 13px;
            font-weight: 800;
            color: #10213f;
            overflow-wrap: anywhere;
        }

        .employee-photo-file-meta {
            color: #64748b;
            font-size: 11px;
            font-weight: 600;
            margin-top: 4px;
        }

        .employee-photo-actions {
            display: grid;
            gap: 8px;
            justify-items: stretch;
            width: 100%;
        }

        .employee-photo-upload-note {
            color: #64748b;
            font-size: 11px;
            line-height: 1.5;
        }

        .employee-photo-empty {
            border: 1px dashed #c9d7fb;
            border-radius: 12px;
            min-height: 84px;
            display: grid;
            grid-template-columns: 1fr;
            gap: 14px;
            align-items: center;
            background: linear-gradient(180deg, #fafcff 0%, #f7f9ff 100%);
            padding: 14px;
            text-align: left;
        }

        .employee-photo-empty-icon {
            width: 44px;
            height: 44px;
            border-radius: 10px;
            background: #e9efff;
            color: #4f46e5;
            display: grid;
            place-items: center;
            font-size: 18px;
        }

        .employee-photo-empty-title {
            font-size: 13px;
            font-weight: 800;
            color: #10213f;
            margin-bottom: 4px;
        }

        .employee-photo-empty-subtitle {
            color: #64748b;
            font-size: 11px;
        }

        .employee-photo-trigger {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 10px;
            min-height: 38px;
            padding: 0 16px;
            border-radius: 10px;
            font-size: 12px;
            font-weight: 800;
            cursor: pointer;
            text-decoration: none;
            width: 100%;
            max-width: 100%;
            box-sizing: border-box;
        }

        .employee-photo-trigger.primary {
            background: linear-gradient(135deg, #4f46e5 0%, #2563eb 100%);
            color: #fff;
            box-shadow: 0 14px 30px rgba(37, 99, 235, 0.2);
        }

        .employee-photo-trigger.secondary {
            background: #ffffff;
            color: #2563eb;
            border: 1px solid #cbd9f6;
        }

        .employee-photo-input {
            display: none;
        }

        .employee-photo-status {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            color: #16a34a;
            font-size: 12px;
            font-weight: 800;
        }

        .employee-photo-status i {
            font-size: 13px;
        }

        .employee-photo-empty-info {
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .employee-photo-modal {
            position: fixed;
            inset: 0;
            background: rgba(15, 23, 42, 0.42);
            backdrop-filter: blur(10px);
            -webkit-backdrop-filter: blur(10px);
            display: none;
            align-items: center;
            justify-content: center;
            z-index: 9999;
            padding: 20px;
        }

        .employee-photo-modal.is-open {
            display: flex;
        }

        .employee-photo-modal-card {
            width: min(560px, 100%);
            background: #ffffff;
            border-radius: 16px;
            box-shadow: 0 28px 60px rgba(15, 23, 42, 0.24);
            overflow: hidden;
        }

        .employee-photo-modal-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 20px 22px;
            border-bottom: 1px solid #edf2f7;
        }

        .employee-photo-modal-title {
            font-size: 18px;
            font-weight: 800;
            color: #10213f;
            margin: 0;
        }

        .employee-photo-modal-close {
            width: 38px;
            height: 38px;
            border-radius: 10px;
            border: 1px solid #d9e3f7;
            background: #ffffff;
            color: #64748b;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            transition: all .18s ease;
        }

        .employee-photo-modal-close:hover {
            background: #fff1f2;
            border-color: #fecdd3;
            color: #dc2626;
        }

        .employee-photo-modal-body {
            padding: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
            min-height: 280px;
            background: #f8fbff;
        }

        .employee-photo-modal-body img {
            max-width: min(320px, 100%);
            max-height: 320px;
            border-radius: 14px;
            box-shadow: 0 18px 40px rgba(15, 23, 42, 0.12);
            background: #ffffff;
            object-fit: contain;
        }

        #certificationFileModal .employee-photo-modal-card {
            width: min(620px, 100%);
        }

        #certificationFileModal .employee-photo-modal-body {
            min-height: 320px;
            padding: 18px;
        }

        #certificationPreviewImage,
        #certificationPreviewFrame {
            width: 100%;
            border: 0;
            border-radius: 14px;
            background: #ffffff;
            box-shadow: 0 18px 40px rgba(15, 23, 42, 0.1);
        }

        #certificationPreviewImage {
            max-width: min(360px, 100%);
            max-height: 360px;
            width: auto;
            height: auto;
            object-fit: contain;
        }

        #certificationPreviewFrame {
            min-height: 420px;
        }

        .is-hidden {
            display: none !important;
        }

        .employee-photo-modal-footer {
            display: flex;
            justify-content: flex-end;
            padding: 18px 22px 22px;
            border-top: 1px solid #edf2f7;
            background: #ffffff;
        }

        .hidden-asset-summary {
            display: none;
        }

        .placeholder-grid {
            display: grid;
            grid-template-columns: repeat(4, minmax(0, 1fr));
            gap: 12px;
        }

        .placeholder-field {
            border: 1px solid #e1e8f2;
            border-radius: 6px;
            padding: 10px 12px;
            min-height: 58px;
            background: #fbfdff;
        }

        .placeholder-label {
            font-size: 11px;
            color: #6b7d99;
            font-weight: 800;
        }

        .placeholder-value {
            color: #1d3151;
            font-size: 13px;
            margin-top: 6px;
        }

        .employee-update-actions {
            border-top: 1px solid #dbe4f0;
            padding-top: 18px;
            margin-top: 16px;
            display: flex;
            justify-content: flex-end;
            gap: 12px;
        }

        .employee-update-actions .btn {
            border-radius: 5px;
            min-height: 44px;
            min-width: 136px;
            font-weight: 800;
            font-size: 13px;
        }

        .employee-update-actions .btn-success {
            background: var(--hrms-primary);
            border-color: var(--hrms-primary);
        }

        .employee-update-actions .btn-primary {
            background: #fff;
            color: #1d3151;
            border: 1px solid #cfdbea;
        }

        .employee-update-actions .btn-secondary {
            background: #fff;
            color: #1d3151;
            border: 1px solid #cfdbea;
        }

        .hidden-password-row {
            display: none;
        }

        @media (max-width: 1200px) {
            .employee-profile-strip,
            .form-grid,
            .form-grid.three,
            .placeholder-grid,
            .personal-documents-grid,
            .personal-section-layout {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }

            .certification-view-grid {
                grid-template-columns: 1fr;
            }

            .contact-view-grid {
                grid-template-columns: 1fr;
            }

            .contact-view-item,
            .contact-view-item:nth-last-child(-n+2) {
                border-bottom: 1px solid #edf2f7;
            }

            .contact-view-grid .contact-view-item:last-child {
                border-bottom: 0;
            }

            .certification-view-item,
            .certification-view-item:nth-child(odd),
            .certification-view-item:nth-child(even) {
                border-right: 0;
                padding-left: 6px;
                padding-right: 6px;
            }

            .certification-view-item:nth-last-child(-n+2) {
                border-bottom: 1px solid #e9eff8;
            }

            .certification-view-item:last-child {
                border-bottom: 0;
            }

            .profile-identity {
                grid-column: 1 / -1;
            }
        }

        @media (max-width: 700px) {
            .employee-update-page {
                padding: 8px 10px 24px;
            }

            .employee-profile-strip,
            .form-grid,
            .form-grid.three,
            .placeholder-grid,
            .personal-documents-grid,
            .personal-section-layout {
                grid-template-columns: 1fr;
            }

            .employee-photo-existing {
                grid-template-columns: 1fr;
            }

            .employee-photo-empty {
                grid-template-columns: 1fr;
            }

            .employee-photo-actions {
                grid-template-columns: 1fr;
            }

            .certification-view-item {
                grid-template-columns: 34px minmax(0, 1fr);
                gap: 10px 12px;
                align-items: start;
                padding: 16px 2px;
            }

            .contact-view-item {
                grid-template-columns: 1fr;
                gap: 4px;
                padding: 14px 2px;
            }

            .certification-view-label {
                grid-column: 2;
                font-size: 11px;
                margin-bottom: 2px;
            }

            .contact-view-label {
                font-size: 11px;
                margin-bottom: 2px;
            }

            .certification-view-colon {
                display: none;
            }

            .contact-view-colon {
                display: none;
            }

            .certification-view-value,
            .certification-view-pill {
                grid-column: 2;
                font-size: 14px;
                min-height: 0;
                padding: 0;
                background: transparent;
                justify-content: flex-start;
            }

            .contact-view-value,
            .contact-view-pill {
                font-size: 14px;
            }

            .certification-file-card {
                flex-direction: column;
                align-items: stretch;
                padding: 16px;
            }

            .certification-file-info {
                align-items: flex-start;
            }

            .certification-file-button {
                width: 100%;
                min-width: 0;
                min-height: 44px;
                font-size: 14px;
            }

            .certification-empty-file {
                text-align: left;
            }

            .profile-stat {
                border-left: 0;
                padding-left: 0;
            }

            .update-section summary {
                flex-wrap: wrap;
                padding: 12px;
            }

            .section-status {
                justify-content: flex-start;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="employee-update-page">
        <div class="employee-breadcrumb">
            <span>Employee List</span>
            <i class="fas fa-chevron-right"></i>
            <span>Employee Update</span>
        </div>

        <div class="employee-title-row">
            <div>
                <asp:Label runat="server" ID="lbluser" CssClass="employee-update-title">Employee Update</asp:Label>
              
            </div>
            <asp:Button ID="btnBack" runat="server" CssClass="btn btn-secondary" Text="Back" OnClick="btnBack_Click" />
        </div>

        <section class="employee-profile-strip">
            <div class="profile-identity">
                <div class="profile-avatar"><i class="fas fa-user"></i></div>
                <div>
                    <div><span id="profileEmployeeCode" class="profile-code">Employee</span><span class="profile-active">Active</span></div>
                    <div id="profileEmployeeName" class="profile-name">Employee Name</div>
                    <div id="profileDesignation" class="profile-meta">Designation</div>
                    <div id="profileDepartment" class="profile-meta">Department | Branch</div>
                </div>
            </div>
            <div class="profile-stat">
                <div class="profile-stat-label">Email</div>
                <div id="profileEmail" class="profile-stat-value">-</div>
            </div>
            <div class="profile-stat">
                <div class="profile-stat-label">Mobile</div>
                <div id="profileMobile" class="profile-stat-value">-</div>
            </div>
            <div class="profile-stat">
                <div class="profile-stat-label">Date of Joining</div>
                <div id="profileJoining" class="profile-stat-value">-</div>
            </div>
            <div class="profile-stat">
                <div class="profile-stat-label">Reporting Manager</div>
                <div id="profileReporting" class="profile-stat-value">-</div>
            </div>
        </section>

        <div class="update-accordion-stack">
            <details class="update-section" open>
                <summary>
                    <span class="section-icon"><i class="fas fa-user"></i></span>
                    <span class="section-title">Personal Information</span>
                    <span class="section-owner">Employee</span>
                    <span class="section-status completed"><i class="fas fa-check-circle"></i> Completed</span>
                    <i class="fas fa-chevron-down section-chevron"></i>
                </summary>
                <div class="section-body">
                    <div class="personal-section-layout">
                        <div class="personal-fields-stack">
                            <div class="form-grid">
                                <div class="field-block">
                                    <label>Employee ID</label>
                                    <asp:TextBox ID="txtEmployeeId" runat="server" CssClass="form-control" Enabled="false" />
                                </div>
                                <div class="field-block">
                                    <label class="required-label">Employee Code</label>
                                    <asp:TextBox ID="txtEmployeeCode" runat="server" CssClass="form-control" MaxLength="15" placeholder="Enter Employee Code" onblur="checkNoLeadingSpace(this)" />
                                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmployeeCode" InitialValue="" ErrorMessage="Employee Code is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>
                                <div class="field-block">
                                    <label class="required-label">Username</label>
                                    <asp:TextBox ID="txt_name" runat="server" CssClass="form-control" MaxLength="15" placeholder="Enter Username" onblur="checkNoLeadingSpace(this)" />
                                    <asp:RequiredFieldValidator ID="rfv_txt_username" runat="server" ControlToValidate="txt_name" InitialValue="" ErrorMessage="Username is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>
                                <div class="field-block">
                                    <label class="required-label">Employee Name</label>
                                    <asp:TextBox ID="txt_fullname" runat="server" CssClass="form-control" MaxLength="30" onkeypress="return blockNumbersAndSpecialChar(event)" onblur="capitalizeFirstLetter(this); checkNoLeadingSpace(this)" placeholder="Enter Employee Name" />
                                    <asp:RequiredFieldValidator ID="rfv_txt_fullname" runat="server" ControlToValidate="txt_fullname" InitialValue="" ErrorMessage="Full Name is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                                </div>
                                <div class="field-block"><label class="required-label">First Name</label><asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter first name" /></div>
                                <div class="field-block"><label>Middle Name</label><asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control" placeholder="Enter middle name" /></div>
                                <div class="field-block"><label class="required-label">Last Name</label><asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter last name" /></div>
                                <div class="field-block"><label class="required-label">Display Name</label><asp:TextBox ID="txtDisplayName" runat="server" CssClass="form-control" placeholder="Enter display name" /></div>
                                <div class="field-block"><label class="required-label">Gender</label><asp:DropDownList ID="txtGender" runat="server" CssClass="form-control custom-dropdown" /></div>
                                <div class="field-block"><label class="required-label">Date of Birth</label><asp:TextBox ID="txtDOB" runat="server" CssClass="form-control" TextMode="Date" /></div>
                                <div class="field-block"><label>Age</label><asp:TextBox ID="txtAge" runat="server" CssClass="form-control" placeholder="Auto calculated" Enabled="false" /></div>
                                <div class="field-block"><label class="required-label">Marital Status</label><asp:DropDownList ID="txtMaritalStatus" runat="server" CssClass="form-control custom-dropdown" /></div>
                                <div class="field-block"><label class="required-label">Blood Group</label><asp:DropDownList ID="txtBloodGroup" runat="server" CssClass="form-control custom-dropdown" /></div>
                                <div class="field-block"><label class="required-label">Nationality</label><asp:DropDownList ID="txtNationality" runat="server" CssClass="form-control custom-dropdown" /></div>
                                <div class="field-block"><label class="required-label">Aadhaar Number</label><asp:TextBox ID="txtAadhaarNumber" runat="server" CssClass="form-control" MaxLength="12" placeholder="12 digit Aadhaar" /></div>
                                <div class="field-block"><label class="required-label">PAN Number</label><asp:TextBox ID="txtPanNumber" runat="server" CssClass="form-control" MaxLength="10" placeholder="ABCDE1234F" /></div>
                                <div class="field-block"><label>Passport Number</label><asp:TextBox ID="txtPassportNumber" runat="server" CssClass="form-control" placeholder="Enter passport number" /></div>
                                <div class="field-block"><label>Passport Expiry Date</label><asp:TextBox ID="txtPassportExpiryDate" runat="server" CssClass="form-control" TextMode="Date" /></div>
                            </div>
                        </div>
                        <div class="photo-card-shell">
                            <div class="photo-card-title">Employee Photograph</div>
                            <input id="fileEmployeePhoto" runat="server" type="file" class="employee-photo-input" accept=".jpg,.jpeg,.png" />

                            <div id="employeePhotoPreviewState" runat="server" class="employee-photo-existing is-hidden">
                                <div class="employee-photo-info">
                                    <div class="employee-photo-file">
                                        <div class="employee-photo-file-icon"><i class="far fa-image"></i></div>
                                        <div>
                                            <asp:Label ID="lblEmployeePhotoFileName" runat="server" CssClass="employee-photo-file-name" />
                                            <div class="employee-photo-status"><i class="fas fa-check-circle"></i> Photo Uploaded Successfully</div>
                                        </div>
                                    </div>
                                    <div class="employee-photo-file-meta">245 KB  .  JPG</div>
                                </div>
                                <div class="employee-photo-actions">
                                    <button type="button" class="employee-photo-trigger secondary" onclick="openEmployeePhotoModal()">
                                        <i class="far fa-eye"></i> View Photo
                                    </button>
                                </div>
                            </div>

                            <div id="employeePhotoEmptyState" runat="server" class="employee-photo-empty is-hidden">
                                <div class="employee-photo-empty-info">
                                    <div class="employee-photo-empty-icon"><i class="fas fa-camera"></i></div>
                                    <div>
                                        <div class="employee-photo-empty-title">Employee Photograph</div>
                                        <div class="employee-photo-empty-subtitle">No photo uploaded yet</div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </details>

            <div id="employeePhotoModal" class="employee-photo-modal" onclick="closeEmployeePhotoModalOnBackdrop(event)">
                <div class="employee-photo-modal-card">
                    <div class="employee-photo-modal-header">
                        <h3 class="employee-photo-modal-title">Employee Photograph</h3>
                        <button type="button" class="employee-photo-modal-close" onclick="closeEmployeePhotoModal()" aria-label="Close Photo Modal">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                    <div class="employee-photo-modal-body">
                        <asp:Image ID="imgEmployeePhotoPreview" runat="server" AlternateText="Employee Photo" />
                    </div>
                    <div class="employee-photo-modal-footer">
                        <button type="button" class="employee-photo-trigger primary" style="width:auto;" onclick="closeEmployeePhotoModal()">Close</button>
                    </div>
                </div>
            </div>

            <div id="certificationFileModal" class="employee-photo-modal" onclick="closeCertificationFileModalOnBackdrop(event)">
                <div class="employee-photo-modal-card">
                    <div class="employee-photo-modal-header">
                        <h3 id="certificationFileModalTitle" class="employee-photo-modal-title">Professional Certificate</h3>
                        <button type="button" class="employee-photo-modal-close" onclick="closeCertificationFileModal()" aria-label="Close Certificate Modal">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                    <div class="employee-photo-modal-body">
                        <img id="certificationPreviewImage" class="is-hidden" alt="Professional Certificate" />
                        <iframe id="certificationPreviewFrame" class="is-hidden" title="Professional Certificate"></iframe>
                    </div>
                    <div class="employee-photo-modal-footer">
                        <button type="button" class="employee-photo-trigger primary" style="width:auto;" onclick="closeCertificationFileModal()">Close</button>
                    </div>
                </div>
            </div>

            <details class="update-section">
                <summary>
                    <span class="section-icon"><i class="fas fa-phone"></i></span>
                    <span class="section-title">Contact Information</span>
                    <span class="section-owner">Employee</span>
                    <span id="contactSectionStatus" runat="server" class="section-status pending"><i class="far fa-clock"></i> Pending</span>
                    <i class="fas fa-chevron-down section-chevron"></i>
                </summary>
                <div class="section-body view-only-section">
                    <div class="form-grid">
                        <div class="field-block">
                            <label class="required-label">Mobile Number</label>
                            <asp:TextBox ID="txt_contact" runat="server" CssClass="form-control" placeholder="Enter Contact Number" onblur="checkNoLeadingSpace(this)" oninput="formatPhoneNumber(this)" />
                            <asp:RequiredFieldValidator ID="rfv_txt_contact" runat="server" ControlToValidate="txt_contact" InitialValue="" ErrorMessage="Contact Number is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                            <span id="officialMobileInlineError" class="inline-field-message"></span>
                        </div>
                        <div class="field-block">
                            <label class="required-label">Official Email ID</label>
                            <asp:TextBox ID="txt_email" runat="server" CssClass="form-control" placeholder="Enter Email Id" onblur="checkNoLeadingSpace(this)" />
                            <asp:RequiredFieldValidator ID="rfv_txt_email" runat="server" ControlToValidate="txt_email" InitialValue="" ErrorMessage="Email Id is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                            <span id="officialEmailInlineError" class="inline-field-message"></span>
                        </div>
                        <div class="field-block"><label>Alternate Mobile Number</label><asp:TextBox ID="txtAlternateMobile" runat="server" CssClass="form-control" placeholder="Enter alternate mobile" /></div>
                        <div class="field-block"><label>Personal Email ID</label><asp:TextBox ID="txtPersonalEmail" runat="server" CssClass="form-control" placeholder="personal@email.com" /></div>
                    </div>
                    <div class="contact-action-row">
                        <asp:Button ID="btnUpdateOfficialContact" runat="server" CssClass="btn btn-primary" Text="Update Contact Details" OnClick="btnUpdateOfficialContact_Click" OnClientClick="return validateOfficialContactClientForm();" CausesValidation="false" Visible="false" />
                    </div>
                    <div class="form-section-title" style="margin-top: 18px;">Permanent Address</div>
                    <div class="form-grid">
                        <div class="field-block"><label>House Number</label><asp:TextBox ID="txtPermanentHouseNumber" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Building Name</label><asp:TextBox ID="txtPermanentBuildingName" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Street</label><asp:TextBox ID="txtPermanentStreet" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Area</label><asp:TextBox ID="txtPermanentArea" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Landmark</label><asp:TextBox ID="txtPermanentLandmark" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>City</label><asp:TextBox ID="txtPermanentCity" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>District</label><asp:TextBox ID="txtPermanentDistrict" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>State</label><asp:TextBox ID="txtPermanentState" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Country</label><asp:TextBox ID="txtPermanentCountry" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>PIN Code</label><asp:TextBox ID="txtPermanentPinCode" runat="server" CssClass="form-control" /></div>
                    </div>
                    <div class="form-section-title" style="margin-top: 18px;">Current Address</div>
                    <label class="readonly-note" style="display:inline-flex; gap:8px; align-items:center; margin-bottom: 14px;"><asp:CheckBox ID="chkSameAsPermanent" runat="server" /> Same as Permanent Address</label>
                    <div class="form-grid">
                        <div class="field-block"><label>House Number</label><asp:TextBox ID="txtCurrentHouseNumber" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Building Name</label><asp:TextBox ID="txtCurrentBuildingName" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Street</label><asp:TextBox ID="txtCurrentStreet" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Area</label><asp:TextBox ID="txtCurrentArea" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Landmark</label><asp:TextBox ID="txtCurrentLandmark" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>City</label><asp:TextBox ID="txtCurrentCity" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>District</label><asp:TextBox ID="txtCurrentDistrict" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>State</label><asp:TextBox ID="txtCurrentState" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Country</label><asp:TextBox ID="txtCurrentCountry" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>PIN Code</label><asp:TextBox ID="txtCurrentPinCode" runat="server" CssClass="form-control" /></div>
                    </div>
                    <div class="form-section-title" style="margin-top: 18px;">Emergency Contact</div>
                    <div class="form-grid">
                        <div class="field-block"><label>Emergency Contact Name</label><asp:TextBox ID="txtEmergencyContactName" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Emergency Contact Number</label><asp:TextBox ID="txtEmergencyContactNumber" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Emergency Contact Relationship</label><asp:TextBox ID="txtEmergencyContactRelationship" runat="server" CssClass="form-control" /></div>
                    </div>
                </div>
            </details>

            <details class="update-section">
                <summary>
                    <span class="section-icon"><i class="fas fa-briefcase"></i></span>
                    <span class="section-title">Employment Information</span>
                    <span class="section-owner">HRMS</span>
                    <span class="section-status completed"><i class="fas fa-check-circle"></i> Completed</span>
                    <i class="fas fa-chevron-down section-chevron"></i>
                </summary>
                <div class="section-body">
                    <div class="form-grid">
                        <div class="field-block">
                            <label class="required-label">Employment Type</label>
                            <asp:DropDownList ID="ddlexporintern" runat="server" CssClass="form-control custom-dropdown" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlexporintern" InitialValue="" ErrorMessage="Type is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                        </div>
                        <div class="field-block">
                            <label class="required-label">Date of Joining</label>
                            <asp:TextBox runat="server" ID="txtDateOfJoining" CssClass="form-control" TextMode="Date" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDateOfJoining" InitialValue="" ErrorMessage="Date is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                        </div>
                        <div class="field-block">
                            <label class="required-label">Probation Period</label>
                            <asp:DropDownList ID="ddlprobationperiod" runat="server" CssClass="form-control">
                                <asp:ListItem Text="0 Month" Value="0"></asp:ListItem>
                                <asp:ListItem Text="1 Month" Value="1"></asp:ListItem>
                                <asp:ListItem Text="3 Months" Value="3"></asp:ListItem>
                                <asp:ListItem Text="6 Months" Value="6" Selected="True"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="field-block"><label>Confirmation Date</label><asp:TextBox ID="txtConfirmationDate" runat="server" CssClass="form-control" TextMode="Date" /></div>
                        <div class="field-block"><label>Probation End Date</label><asp:TextBox ID="txtProbationEndDate" runat="server" CssClass="form-control" TextMode="Date" /></div>
                        <div class="field-block"><label class="required-label">Employee Status</label><asp:DropDownList ID="txtEmployeeStatus" runat="server" CssClass="form-control custom-dropdown" /></div>
                        <div class="field-block"><label>Notice Period</label><asp:TextBox ID="txtNoticePeriod" runat="server" CssClass="form-control" placeholder="Days" /></div>
                        <div class="field-block"><label>Exit Date</label><asp:TextBox ID="txtExitDate" runat="server" CssClass="form-control" TextMode="Date" /></div>
                        <div class="field-block"><label>Separation Reason</label><asp:DropDownList ID="txtSeparationReason" runat="server" CssClass="form-control custom-dropdown" /></div>
                        <div class="field-block"><label>Probation Status</label><asp:TextBox ID="txtProbationStatus" runat="server" CssClass="form-control" /></div>
                        <asp:TextBox ID="txtProbationRemarks" runat="server" CssClass="form-control" style="display:none;" />
                    </div>
                </div>
            </details>

            <details class="update-section">
                <summary>
                    <span class="section-icon"><i class="fas fa-building"></i></span>
                    <span class="section-title">Organization Details</span>
                    <span class="section-owner">HRMS</span>
                    <span class="section-status completed"><i class="fas fa-check-circle"></i> Completed</span>
                    <i class="fas fa-chevron-down section-chevron"></i>
                </summary>
                <div class="section-body">
                    <div class="form-grid">
                        <div class="field-block">
                            <label class="required-label">Company</label>
                            <asp:DropDownList ID="ddlcompany" runat="server" CssClass="form-control custom-dropdown" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddlcompany" InitialValue="" ErrorMessage="Company Name is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                        </div>
                        <div class="field-block">
                            <label class="required-label">Department</label>
                            <asp:TextBox ID="txtdept" runat="server" CssClass="form-control" MaxLength="50" placeholder="Enter Department Name" onblur="checkNoLeadingSpace(this)" />
                            <asp:RequiredFieldValidator ID="rfv_txtdept" runat="server" ControlToValidate="txtdept" InitialValue="" ErrorMessage="Department is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                        </div>
                        <div class="field-block">
                            <label class="required-label">Branch Office</label>
                            <asp:TextBox ID="txtbranch" runat="server" CssClass="form-control" MaxLength="50" placeholder="Enter Branch" onblur="checkNoLeadingSpace(this)" />
                            <asp:RequiredFieldValidator ID="rfv_txtbranch" runat="server" ControlToValidate="txtbranch" InitialValue="" ErrorMessage="Branch is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                        </div>
                        <div class="field-block">
                            <label class="required-label">Designation</label>
                            <asp:DropDownList ID="ddldesign" runat="server" CssClass="form-control custom-dropdown" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="ddldesign" InitialValue="" ErrorMessage="Designation is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                        </div>
                        <div class="field-block">
                            <label class="required-label">Reporting Manager</label>
                            <asp:DropDownList ID="ddl_reportingmanager" runat="server" CssClass="form-control custom-dropdown" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddl_reportingmanager" InitialValue="" ErrorMessage="Reporting Manager is required" ForeColor="Red" Display="Dynamic" ValidationGroup="SaveValidationGroup" />
                        </div>
                        <div class="field-block"><label class="required-label">Location</label><asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" placeholder="Enter work location" /></div>
                        <div class="field-block"><label class="required-label">Functional Manager</label><asp:TextBox ID="txtFunctionalManager" runat="server" CssClass="form-control" placeholder="Functional manager" /></div>
                        <div class="field-block"><label class="required-label">HOD</label><asp:TextBox ID="txtHod" runat="server" CssClass="form-control" placeholder="Head of department" /></div>
                        <div class="field-block"><label>Employee Level</label><asp:DropDownList ID="txtEmployeeLevel" runat="server" CssClass="form-control custom-dropdown" /></div>
                    </div>
                </div>
            </details>

            <details class="update-section">
                <summary>
                    <span class="section-icon"><i class="fas fa-clock"></i></span>
                    <span class="section-title">Attendance & Shift Information</span>
                    <span class="section-owner">HRMS</span>
                    <span id="attendanceSectionStatus" runat="server" class="section-status pending"><i class="far fa-clock"></i> Pending</span>
                    <i class="fas fa-chevron-down section-chevron"></i>
                </summary>
                <div class="section-body">
                    <div class="form-grid">
                        <div class="field-block"><label class="required-label">Attendance Type</label><asp:DropDownList ID="txtAttendanceType" runat="server" CssClass="form-control custom-dropdown" /></div>
                        <div class="field-block"><label class="required-label">Weekly Off</label><asp:DropDownList ID="txtWeeklyOff" runat="server" CssClass="form-control custom-dropdown" /></div>
                        <div class="field-block"><label class="required-label">Working Hours</label><asp:TextBox ID="txtWorkingHours" runat="server" CssClass="form-control" placeholder="09:30" /></div>
                        <div class="field-block"><label class="required-label">Punching Device ID</label><asp:TextBox ID="txtPunchingDeviceId" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label class="required-label">Biometric ID</label><asp:TextBox ID="txtBiometricId" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label class="required-label">Attendance Policy</label><asp:DropDownList ID="txtAttendancePolicy" runat="server" CssClass="form-control custom-dropdown" /></div>
                        <div class="field-block">
                            <label class="required-label">Overtime Eligible</label>
                            <asp:HiddenField ID="hdnOvertimeEligible" runat="server" />
                            <div class="switch-field">
                                <span class="switch-label">No</span>
                                <label class="yes-no-switch">
                                    <input type="checkbox" id="chkOvertimeEligible" onchange="toggleOvertimeUi()" />
                                    <span class="yes-no-slider"></span>
                                </label>
                                <span class="switch-label">Yes</span>
                            </div>
                        </div>
                        <div class="field-block"><label>Overtime Rate</label><asp:TextBox ID="txtOvertimeRate" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label class="required-label">Work Location</label><asp:DropDownList ID="txtWorkLocation" runat="server" CssClass="form-control custom-dropdown" /></div>
                    </div>
                </div>
            </details>

            <details class="update-section">
                <summary>
                    <span class="section-icon"><i class="fas fa-university"></i></span>
                    <span class="section-title">Bank Details</span>
                    <span class="section-owner">Employee</span>
                    <span id="bankSectionStatus" runat="server" class="section-status pending"><i class="far fa-clock"></i> Pending</span>
                    <i class="fas fa-chevron-down section-chevron"></i>
                </summary>
                <div class="section-body view-only-section">
                    <div class="form-grid">
                        <div class="field-block"><label>Bank Name</label><asp:TextBox ID="txtBankName" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Branch Name</label><asp:TextBox ID="txtBankBranchName" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Account Holder Name</label><asp:TextBox ID="txtAccountHolderName" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Account Number</label><asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Confirm Account Number</label><asp:TextBox ID="txtConfirmAccountNumber" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>IFSC Code</label><asp:TextBox ID="txtIfscCode" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Account Type</label><asp:TextBox ID="txtAccountType" runat="server" CssClass="form-control" /></div>
                        <div class="field-block"><label>Salary Account Flag</label><asp:TextBox ID="txtSalaryAccountFlag" runat="server" CssClass="form-control" /></div>
                    </div>
                </div>
            </details>

            <details class="update-section">
                <summary>
                    <span class="section-icon"><i class="fas fa-graduation-cap"></i></span>
                    <span class="section-title">Educational Qualifications</span>
                    <span class="section-owner">Employee</span>
                    <span id="educationSectionStatus" runat="server" class="section-status pending"><i class="far fa-clock"></i> Pending</span>
                    <i class="fas fa-chevron-down section-chevron"></i>
                </summary>
                <div class="section-body">
                    <div class="managed-entry-block">
                        <div class="managed-entry-header">
                            <div>
                                <h3 class="managed-entry-title">Education</h3>
                                
                            </div>
                        </div>

                        <div class="entry-table-wrap">
                            <table class="entry-table">
                                <thead>
                                    <tr>
                                        <th><i class="far fa-file-alt" style="margin-right:8px;"></i>Qualification Level</th>
                                        <th><i class="far fa-file-alt" style="margin-right:8px;"></i>Degree Name</th>
                                        <th><i class="fas fa-graduation-cap" style="margin-right:8px;"></i>Specialization</th>
                                        <th><i class="fas fa-university" style="margin-right:8px;"></i>University</th>
                                        <th><i class="fas fa-school" style="margin-right:8px;"></i>Institute Name</th>
                                        <th><i class="far fa-calendar-alt" style="margin-right:8px;"></i>Year of Passing</th>
                                        <th><i class="fas fa-percent" style="margin-right:8px;"></i>Percentage / CGPA</th>
                                        <th><i class="far fa-file-pdf" style="margin-right:8px;"></i>Certificate</th>
                                    </tr>
                                </thead>
                                <tbody id="educationPreviewBody" runat="server">
                                    <tr id="educationEmptyRow">
                                        <td colspan="8" class="entry-empty">No education added yet.</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div id="educationEditorCard" class="entry-editor-card collapsed" style="display:none !important;">
                        <asp:HiddenField ID="hdnEducationId" runat="server" Value="0" />
                        <div class="entry-editor-header">
                            <div class="entry-editor-title-wrap">
                                <div class="entry-editor-badge"><i class="fas fa-graduation-cap"></i></div>
                                <div>
                                    <h4 class="entry-editor-title">Add / Edit Education Details</h4>
                                    <p class="entry-editor-subtitle">Enter the education information below.</p>
                                </div>
                            </div>
                            <button type="button" class="asset-editor-close" onclick="hideSectionEditor('educationEditorCard')" title="Close Education Form" aria-label="Close Education Form">
                                <i class="fas fa-times"></i>
                            </button>
                        </div>
                        <div class="entry-editor-body">
                            <div class="form-grid">
                                <div class="field-block"><label>Qualification Level</label><asp:TextBox ID="txtQualificationLevel" runat="server" CssClass="form-control" /></div>
                                <div class="field-block"><label>Degree Name</label><asp:TextBox ID="txtDegreeName" runat="server" CssClass="form-control" /></div>
                                <div class="field-block"><label>Specialization</label><asp:TextBox ID="txtSpecialization" runat="server" CssClass="form-control" /></div>
                                <div class="field-block"><label>University</label><asp:TextBox ID="txtUniversity" runat="server" CssClass="form-control" /></div>
                                <div class="field-block"><label>Institute Name</label><asp:TextBox ID="txtInstituteName" runat="server" CssClass="form-control" /></div>
                                <div class="field-block"><label>Year of Passing</label><asp:TextBox ID="txtYearOfPassing" runat="server" CssClass="form-control" /></div>
                                <div class="field-block"><label>Percentage / CGPA</label><asp:TextBox ID="txtPercentageCgpa" runat="server" CssClass="form-control" /></div>
                                <div class="field-block">
                                    <label>Upload Certificate</label>
                                    <input id="fileEducationCertificate" runat="server" type="file" class="form-control" />
                                    <asp:HyperLink ID="lnkEducationCertificate" runat="server" CssClass="document-view-link" Target="_blank" Visible="false">
                                        <i class="far fa-eye"></i> View
                                    </asp:HyperLink>
                                </div>
                            </div>
                        </div>
                        <div class="entry-editor-actions">
                            <asp:Button ID="btnSaveEducation" runat="server" CssClass="btn btn-primary" Text="Save Education" OnClick="btnSaveEducation_Click" CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </details>

            <details class="update-section">
                <summary>
                    <span class="section-icon"><i class="fas fa-award"></i></span>
                    <span class="section-title">Professional Certifications</span>
                    <span class="section-owner">Employee</span>
                    <span id="certificationSectionStatus" runat="server" class="section-status pending"><i class="far fa-clock"></i> Pending</span>
                    <i class="fas fa-chevron-down section-chevron"></i>
                </summary>
                <div class="section-body">
                    <asp:TextBox ID="txtCertificationName" runat="server" CssClass="form-control" style="display:none;" />
                    <asp:TextBox ID="txtCertificationAuthority" runat="server" CssClass="form-control" style="display:none;" />
                    <asp:TextBox ID="txtCertificateNumber" runat="server" CssClass="form-control" style="display:none;" />
                    <asp:TextBox ID="txtIssueDate" runat="server" CssClass="form-control" style="display:none;" />
                    <asp:TextBox ID="txtExpiryDate" runat="server" CssClass="form-control" style="display:none;" />
                    <asp:TextBox ID="txtRenewalRequired" runat="server" CssClass="form-control" style="display:none;" />
                    <input id="fileCertification" runat="server" type="file" class="form-control" style="display:none;" />

                    <div class="certification-view-card">
                        <div class="certification-view-grid">
                            <div class="certification-view-item">
                                <span class="certification-view-icon"><i class="far fa-id-badge"></i></span>
                                <span class="certification-view-label">Certification Name</span>
                                <span class="certification-view-colon">:</span>
                                <asp:Label ID="lblCertificationName" runat="server" CssClass="certification-view-value" />
                            </div>
                            <div class="certification-view-item">
                                <span class="certification-view-icon"><i class="fas fa-university"></i></span>
                                <span class="certification-view-label">Certification Authority</span>
                                <span class="certification-view-colon">:</span>
                                <asp:Label ID="lblCertificationAuthority" runat="server" CssClass="certification-view-value" />
                            </div>
                            <div class="certification-view-item">
                                <span class="certification-view-icon"><i class="fas fa-hashtag"></i></span>
                                <span class="certification-view-label">Certificate Number</span>
                                <span class="certification-view-colon">:</span>
                                <asp:Label ID="lblCertificateNumber" runat="server" CssClass="certification-view-value" />
                            </div>
                            <div class="certification-view-item">
                                <span class="certification-view-icon"><i class="far fa-calendar-alt"></i></span>
                                <span class="certification-view-label">Issue Date</span>
                                <span class="certification-view-colon">:</span>
                                <asp:Label ID="lblCertificationIssueDate" runat="server" CssClass="certification-view-value" />
                            </div>
                            <div class="certification-view-item">
                                <span class="certification-view-icon"><i class="far fa-calendar"></i></span>
                                <span class="certification-view-label">Expiry Date</span>
                                <span class="certification-view-colon">:</span>
                                <asp:Label ID="lblCertificationExpiryDate" runat="server" CssClass="certification-view-value" />
                            </div>
                            <div class="certification-view-item">
                                <span class="certification-view-icon"><i class="fas fa-sync-alt"></i></span>
                                <span class="certification-view-label">Renewal Required</span>
                                <span class="certification-view-colon">:</span>
                                <asp:Label ID="lblRenewalRequired" runat="server" CssClass="certification-view-pill" />
                            </div>
                        </div>

                        <div class="certification-file-panel">
                            <div class="certification-file-card">
                                <div class="certification-file-info">
                                    <div class="certification-file-icon"><i class="far fa-file-pdf"></i></div>
                                    <div class="certification-file-meta">
                                        <div class="certification-file-label">Certificate File</div>
                                        <asp:Label ID="lblCertificationFileName" runat="server" CssClass="certification-file-name" />
                                        <div class="certification-file-submeta"><asp:Label ID="lblCertificationFileMeta" runat="server" /></div>
                                    </div>
                                </div>
                                <asp:HyperLink ID="lnkCertificationFile" runat="server" CssClass="certification-file-button" Target="_blank" Visible="false">
                                    <i class="far fa-eye"></i> View Certificate
                                </asp:HyperLink>
                                <asp:Label ID="lblCertificationNoFile" runat="server" CssClass="certification-empty-file" Visible="false" Text="No certificate uploaded." />
                            </div>
                        </div>
                    </div>

                    
                </div>
            </details>

            <details class="update-section">
                <summary>
                    <span class="section-icon"><i class="fas fa-briefcase"></i></span>
                    <span class="section-title">Work Experience</span>
                    <span class="section-owner">Employee</span>
                    <span id="workExperienceSectionStatus" runat="server" class="section-status pending"><i class="far fa-clock"></i> Pending</span>
                    <i class="fas fa-chevron-down section-chevron"></i>
                </summary>
                <div class="section-body">
                    <div class="managed-entry-block">
                        

                        <div class="entry-table-wrap">
                            <table class="entry-table">
                                <thead>
                                    <tr>
                                        <th><i class="far fa-building" style="margin-right:8px;"></i>Organization Name</th>
                                        <th><i class="fas fa-layer-group" style="margin-right:8px;"></i>Industry</th>
                                        <th><i class="fas fa-briefcase" style="margin-right:8px;"></i>Designation</th>
                                        <th><i class="far fa-clock" style="margin-right:8px;"></i>Employment Period</th>
                                        <th><i class="far fa-calendar-alt" style="margin-right:8px;"></i>Start Date</th>
                                        <th><i class="far fa-calendar-alt" style="margin-right:8px;"></i>End Date</th>
                                        <th><i class="fas fa-history" style="margin-right:8px;"></i>Total Experience</th>
                                        <th><i class="fas fa-rupee-sign" style="margin-right:8px;"></i>Last Drawn Salary</th>
                                    </tr>
                                </thead>
                                <tbody id="workExperiencePreviewBody" runat="server">
                                    <tr id="workExperienceEmptyRow">
                                        <td colspan="8" class="entry-empty">No work experience added yet.</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div id="workExperienceEditorCard" class="entry-editor-card collapsed" style="display:none !important;">
                        <asp:HiddenField ID="hdnWorkExperienceId" runat="server" Value="0" />
                        <div class="entry-editor-header">
                            <div class="entry-editor-title-wrap">
                                <div class="entry-editor-badge"><i class="fas fa-briefcase"></i></div>
                                <div>
                                    <h4 class="entry-editor-title">Add / Edit Work Experience</h4>
                                    <p class="entry-editor-subtitle">Enter the previous employment details below.</p>
                                </div>
                            </div>
                            <button type="button" class="asset-editor-close" onclick="hideSectionEditor('workExperienceEditorCard')" title="Close Work Experience Form" aria-label="Close Work Experience Form">
                                <i class="fas fa-times"></i>
                            </button>
                        </div>
                        <div class="entry-editor-body">
                            <div class="form-grid">
                                <div class="field-block"><label>Organization Name</label><input type="text" id="txtWorkOrganizationName" name="txtWorkOrganizationName" class="form-control" /></div>
                                <div class="field-block"><label>Industry</label><input type="text" id="txtWorkIndustry" name="txtWorkIndustry" class="form-control" /></div>
                                <div class="field-block"><label>Designation</label><input type="text" id="txtWorkDesignation" name="txtWorkDesignation" class="form-control" /></div>
                                <div class="field-block"><label>Employment Period</label><input type="text" id="txtWorkEmploymentPeriod" name="txtWorkEmploymentPeriod" class="form-control" placeholder="Years / months" /></div>
                                <div class="field-block"><label>Start Date</label><input type="date" id="txtWorkStartDate" name="txtWorkStartDate" class="form-control" /></div>
                                <div class="field-block"><label>End Date</label><input type="date" id="txtWorkEndDate" name="txtWorkEndDate" class="form-control" /></div>
                                <div class="field-block"><label>Total Experience</label><input type="text" id="txtWorkTotalExperience" name="txtWorkTotalExperience" class="form-control" /></div>
                                <div class="field-block"><label>Last Drawn Salary</label><input type="number" step="0.01" id="txtWorkLastDrawnSalary" name="txtWorkLastDrawnSalary" class="form-control" /></div>
                            </div>
                        </div>
                        <div class="entry-editor-actions">
                            <asp:Button ID="btnSaveWorkExperience" runat="server" CssClass="btn btn-primary" Text="Save Work Experience" OnClick="btnSaveWorkExperience_Click" CausesValidation="false" />
                        </div>
                    </div>
                </div>
            </details>

            <details class="update-section">
                <summary>
                    <span class="section-icon"><i class="fas fa-desktop"></i></span>
                    <span class="section-title">Assets Assigned</span>
                    <span class="section-owner">HRMS</span>
                    <span class="section-status completed"><i class="fas fa-check-circle"></i> Completed</span>
                    <i class="fas fa-chevron-down section-chevron"></i>
                </summary>
                <div class="section-body">
                    <div class="managed-entry-block">
                        <div class="managed-entry-header">
                            <div>
                                <h3 class="managed-entry-title">Assets Assigned</h3>
                                
                            </div>
                            <button type="button" class="managed-entry-button" onclick="showAssetEditor(false)">
                                <i class="fas fa-plus"></i>
                                <span>Add Asset</span>
                            </button>
                        </div>

                        <div class="asset-table-wrap">
                            <asp:GridView ID="gvEmployeeAssets" runat="server" AutoGenerateColumns="False" CssClass="asset-table" GridLines="None" EmptyDataText="No assets assigned." DataKeyNames="AssetAssignmentId" OnRowCommand="gvEmployeeAssets_RowCommand">
                                <Columns>
                                    <asp:BoundField HeaderText="Asset Type" DataField="AssetType" />
                                    <asp:BoundField HeaderText="Asset Number" DataField="AssetNumber" />
                                    <asp:BoundField HeaderText="Asset Name" DataField="AssetName" />
                                    <asp:BoundField HeaderText="Assigned Date" DataField="AssignedDate" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" />
                                    <asp:BoundField HeaderText="Return Date" DataField="ReturnDate" DataFormatString="{0:dd-MM-yyyy}" HtmlEncode="False" NullDisplayText="-" />
                                    <asp:BoundField HeaderText="Asset Condition" DataField="AssetCondition" />
                                    <asp:BoundField HeaderText="Asset Status" DataField="AssetStatus" />
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <div class="asset-action-stack">
                                                <button type="button" class="asset-action-btn" title="Edit Asset"
                                                    data-id='<%# Eval("AssetAssignmentId") %>'
                                                    data-type='<%# Eval("AssetType") %>'
                                                    data-number='<%# Eval("AssetNumber") %>'
                                                    data-name='<%# Eval("AssetName") %>'
                                                    data-assigned='<%# Eval("AssignedDate", "{0:dd-MM-yyyy}") %>'
                                                    data-return='<%# Eval("ReturnDate", "{0:dd-MM-yyyy}") %>'
                                                    data-condition-id='<%# Eval("AssetConditionId") %>'
                                                    data-status-id='<%# Eval("AssetStatusId") %>'
                                                    onclick="openAssetEditorForEdit(this)">
                                                    <i class="far fa-edit"></i>
                                                </button>
                                                <asp:LinkButton ID="lnkDeleteAsset" runat="server" CssClass="asset-action-btn danger" CommandName="deleteAsset" CommandArgument='<%# Eval("AssetAssignmentId") %>' title="Delete Asset" CausesValidation="false">
                                                    <i class="far fa-trash-alt"></i>
                                                </asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                    <div id="assetEditorCard" runat="server" class="entry-editor-card asset-editor-card collapsed">
                        <asp:HiddenField ID="hdnAssetAssignmentId" runat="server" Value="0" />
                        <div class="entry-editor-header asset-editor-header">
                            <div class="entry-editor-title-wrap">
                                <div class="entry-editor-badge"><i class="fas fa-desktop"></i></div>
                                <div>
                                    <h4 class="entry-editor-title">Add / Edit Asset Details</h4>
                                </div>
                            </div>
                            <button type="button" class="asset-editor-close" onclick="hideAssetEditor()" title="Close Asset Form" aria-label="Close Asset Form">
                                <i class="fas fa-times"></i>
                            </button>
                        </div>
                        <div class="entry-editor-body">
                            <div class="form-grid">
                                <div class="field-block"><label class="required-label">Asset Type</label><asp:TextBox ID="txtAssetType" runat="server" CssClass="form-control" placeholder="Laptop / Mobile Phone / SIM Card" /></div>
                                <div class="field-block"><label class="required-label">Asset Number</label><asp:TextBox ID="txtAssetNumber" runat="server" CssClass="form-control" /></div>
                                <div class="field-block"><label class="required-label">Asset Name</label><asp:TextBox ID="txtAssetName" runat="server" CssClass="form-control" /></div>
                                <div class="field-block"><label class="required-label">Assigned Date</label><asp:TextBox ID="txtAssignedDate" runat="server" CssClass="form-control" TextMode="Date" /></div>
                                <div class="field-block"><label>Return Date</label><asp:TextBox ID="txtReturnDate" runat="server" CssClass="form-control" TextMode="Date" /></div>
                                <div class="field-block"><label class="required-label">Asset Condition</label><asp:DropDownList ID="ddlAssetCondition" runat="server" CssClass="form-control"></asp:DropDownList></div>
                                <div class="field-block"><label class="required-label">Asset Status</label><asp:DropDownList ID="ddlAssetStatus" runat="server" CssClass="form-control"></asp:DropDownList></div>
                            </div>
                        </div>
                        <div class="entry-editor-actions asset-editor-actions">
                            <asp:Button ID="btnSaveAsset" runat="server" CssClass="btn btn-primary" Text="Add Asset" OnClick="btnSaveAsset_Click" CausesValidation="false" OnClientClick="return validateAssetClientForm();" />
                        </div>
                    </div>
                </div>
            </details>

        </div>

        <div class="hidden-password-row">
            <asp:Label runat="server" ID="lblpass">Password</asp:Label>
            <asp:TextBox ID="txt_password" runat="server" TextMode="Password" />
            <asp:TextBox ID="txtESICNo" runat="server" Text="0" />
            <asp:TextBox ID="txtPFNo" runat="server" Text="0" />
        </div>

        <div class="employee-update-actions">
            <asp:Button CssClass="btn btn-primary custom-clear-button" runat="server" ID="btn_rest" Text="Cancel" OnClick="ClearButton_Click" />
            <asp:Button ID="btn_submit" runat="server" CssClass="btn btn-success" ValidationGroup="SaveValidationGroup" OnClick="SubmitButtonClick" Text="Update" CommandArgument="Submit" OnClientClick="return validateAddEmployeeClientForm();" />
        </div>
    </div>

    <script>
        function showUserSavedMessage(status, remark) {
            Swal.fire({
                icon: status === "Success" ? "success" : "error",
                text: remark,
                timer: 4000,
                showConfirmButton: false
            });
        }

        function showAssetEditor(isEdit) {
            var editor = document.getElementById('<%= assetEditorCard.ClientID %>');
            if (editor) {
                editor.classList.remove('collapsed');
                editor.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }

            if (!isEdit) {
                clearAssetEditor();
            }
        }

        function hideAssetEditor() {
            var editor = document.getElementById('<%= assetEditorCard.ClientID %>');
            if (editor) {
                editor.classList.add('collapsed');
            }
        }

        function clearAssetEditor() {
            var setValue = function (id, value) {
                var element = document.getElementById(id);
                if (element) {
                    element.value = value;
                }
            };

            setValue('<%= hdnAssetAssignmentId.ClientID %>', '0');
            setValue('<%= txtAssetType.ClientID %>', '');
            setValue('<%= txtAssetNumber.ClientID %>', '');
            setValue('<%= txtAssetName.ClientID %>', '');
            setValue('<%= txtAssignedDate.ClientID %>', '');
            setValue('<%= txtReturnDate.ClientID %>', '');
            setValue('<%= ddlAssetCondition.ClientID %>', '');
            setValue('<%= ddlAssetStatus.ClientID %>', '');

            var saveButton = document.getElementById('<%= btnSaveAsset.ClientID %>');
            if (saveButton) {
                saveButton.value = 'Add Asset';
            }
        }

        function openAssetEditorForEdit(button) {
            if (!button) return;

            var setValue = function (id, value) {
                var element = document.getElementById(id);
                if (element) {
                    element.value = value || '';
                }
            };

            setValue('<%= hdnAssetAssignmentId.ClientID %>', button.getAttribute('data-id'));
            setValue('<%= txtAssetType.ClientID %>', button.getAttribute('data-type'));
            setValue('<%= txtAssetNumber.ClientID %>', button.getAttribute('data-number'));
            setValue('<%= txtAssetName.ClientID %>', button.getAttribute('data-name'));
            setValue('<%= txtAssignedDate.ClientID %>', button.getAttribute('data-assigned'));
            setValue('<%= txtReturnDate.ClientID %>', button.getAttribute('data-return'));
            setValue('<%= ddlAssetCondition.ClientID %>', button.getAttribute('data-condition-id'));
            setValue('<%= ddlAssetStatus.ClientID %>', button.getAttribute('data-status-id'));

            var saveButton = document.getElementById('<%= btnSaveAsset.ClientID %>');
            if (saveButton) {
                saveButton.value = 'Update Asset';
            }

            showAssetEditor(true);
        }

        function showSectionEditor(editorId) {
            var editor = document.getElementById(editorId);
            if (editor) {
                editor.classList.remove('collapsed');
                editor.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }
        }

        function hideSectionEditor(editorId) {
            var editor = document.getElementById(editorId);
            if (editor) {
                editor.classList.add('collapsed');
            }
        }

        function getDisplayValue(value) {
            var text = (value || '').toString().trim();
            return text || '-';
        }

        function removeEmptyRow(rowId) {
            var row = document.getElementById(rowId);
            if (row) {
                row.remove();
            }
        }

        function appendPreviewRow(tbodyId, values) {
            var body = document.getElementById(tbodyId);
            if (!body) return;

            var row = document.createElement('tr');
            for (var i = 0; i < values.length; i++) {
                var cell = document.createElement('td');
                cell.textContent = getDisplayValue(values[i]);
                row.appendChild(cell);
            }

            body.appendChild(row);
        }

        function clearEducationEditor() {
            document.getElementById('<%= hdnEducationId.ClientID %>').value = '0';
            document.getElementById('<%= txtQualificationLevel.ClientID %>').value = '';
            document.getElementById('<%= txtDegreeName.ClientID %>').value = '';
            document.getElementById('<%= txtSpecialization.ClientID %>').value = '';
            document.getElementById('<%= txtUniversity.ClientID %>').value = '';
            document.getElementById('<%= txtInstituteName.ClientID %>').value = '';
            document.getElementById('<%= txtYearOfPassing.ClientID %>').value = '';
            document.getElementById('<%= txtPercentageCgpa.ClientID %>').value = '';

            var educationLink = document.getElementById('<%= lnkEducationCertificate.ClientID %>');
            if (educationLink) {
                educationLink.style.display = 'none';
                educationLink.setAttribute('href', '');
            }

            var educationFile = document.getElementById('<%= fileEducationCertificate.ClientID %>');
            if (educationFile) {
                educationFile.style.display = '';
                educationFile.value = '';
            }
        }

        function openEducationEditorForAdd() {
            clearEducationEditor();
            showSectionEditor('educationEditorCard');
        }

        function syncEducationPreview() {
            removeEmptyRow('educationEmptyRow');

            var certificateLink = document.getElementById('<%= lnkEducationCertificate.ClientID %>');
            var certificateValue = (certificateLink && certificateLink.getAttribute('href')) ? 'View' : 'Upload';

            appendPreviewRow('educationPreviewBody', [
                document.getElementById('<%= txtQualificationLevel.ClientID %>').value,
                document.getElementById('<%= txtDegreeName.ClientID %>').value,
                document.getElementById('<%= txtSpecialization.ClientID %>').value,
                document.getElementById('<%= txtUniversity.ClientID %>').value,
                document.getElementById('<%= txtInstituteName.ClientID %>').value,
                document.getElementById('<%= txtYearOfPassing.ClientID %>').value,
                document.getElementById('<%= txtPercentageCgpa.ClientID %>').value,
                certificateValue
            ]);

            clearEducationEditor();
        }

        function clearWorkExperienceEditor() {
            document.getElementById('<%= hdnWorkExperienceId.ClientID %>').value = '0';
            document.getElementById('txtWorkOrganizationName').value = '';
            document.getElementById('txtWorkIndustry').value = '';
            document.getElementById('txtWorkDesignation').value = '';
            document.getElementById('txtWorkEmploymentPeriod').value = '';
            document.getElementById('txtWorkStartDate').value = '';
            document.getElementById('txtWorkEndDate').value = '';
            document.getElementById('txtWorkTotalExperience').value = '';
            document.getElementById('txtWorkLastDrawnSalary').value = '';
        }

        function openWorkExperienceEditorForAdd() {
            clearWorkExperienceEditor();
            showSectionEditor('workExperienceEditorCard');
        }

        function openEducationEditorForEdit(button) {
            if (!button) return;

            document.getElementById('<%= hdnEducationId.ClientID %>').value = button.getAttribute('data-id') || '0';
            document.getElementById('<%= txtQualificationLevel.ClientID %>').value = button.getAttribute('data-qualification') || '';
            document.getElementById('<%= txtDegreeName.ClientID %>').value = button.getAttribute('data-degree') || '';
            document.getElementById('<%= txtSpecialization.ClientID %>').value = button.getAttribute('data-specialization') || '';
            document.getElementById('<%= txtUniversity.ClientID %>').value = button.getAttribute('data-university') || '';
            document.getElementById('<%= txtInstituteName.ClientID %>').value = button.getAttribute('data-institute') || '';
            document.getElementById('<%= txtYearOfPassing.ClientID %>').value = button.getAttribute('data-year') || '';
            document.getElementById('<%= txtPercentageCgpa.ClientID %>').value = button.getAttribute('data-percentage') || '';

            var certificateLink = document.getElementById('<%= lnkEducationCertificate.ClientID %>');
            var certificateFile = document.getElementById('<%= fileEducationCertificate.ClientID %>');
            var certificateUrl = button.getAttribute('data-certificate-url') || '';
            if (certificateLink) {
                certificateLink.style.display = certificateUrl ? 'inline-flex' : 'none';
                certificateLink.setAttribute('href', certificateUrl);
            }

            if (certificateFile) {
                certificateFile.style.display = certificateUrl ? 'none' : '';
                certificateFile.value = '';
            }

            showSectionEditor('educationEditorCard');
        }

        function openWorkExperienceEditorForEdit(button) {
            if (!button) return;

            document.getElementById('<%= hdnWorkExperienceId.ClientID %>').value = button.getAttribute('data-id') || '0';
            document.getElementById('txtWorkOrganizationName').value = button.getAttribute('data-organization') || '';
            document.getElementById('txtWorkIndustry').value = button.getAttribute('data-industry') || '';
            document.getElementById('txtWorkDesignation').value = button.getAttribute('data-designation') || '';
            document.getElementById('txtWorkEmploymentPeriod').value = button.getAttribute('data-employment-period') || '';
            document.getElementById('txtWorkStartDate').value = button.getAttribute('data-start-date') || '';
            document.getElementById('txtWorkEndDate').value = button.getAttribute('data-end-date') || '';
            document.getElementById('txtWorkTotalExperience').value = button.getAttribute('data-total-experience') || '';
            document.getElementById('txtWorkLastDrawnSalary').value = button.getAttribute('data-last-salary') || '';
            showSectionEditor('workExperienceEditorCard');
        }

        function syncWorkExperiencePreview() {
            removeEmptyRow('workExperienceEmptyRow');

            appendPreviewRow('workExperiencePreviewBody', [
                document.getElementById('txtWorkOrganizationName').value,
                document.getElementById('txtWorkIndustry').value,
                document.getElementById('txtWorkDesignation').value,
                document.getElementById('txtWorkEmploymentPeriod').value,
                document.getElementById('txtWorkStartDate').value,
                document.getElementById('txtWorkEndDate').value,
                document.getElementById('txtWorkTotalExperience').value,
                document.getElementById('txtWorkLastDrawnSalary').value
            ]);

            clearWorkExperienceEditor();
        }

        function toggleOvertimeUi() {
            var toggle = document.getElementById('chkOvertimeEligible');
            var hidden = document.getElementById('<%= hdnOvertimeEligible.ClientID %>');
            var rate = document.getElementById('<%= txtOvertimeRate.ClientID %>');
            if (!toggle || !hidden || !rate) return;

            hidden.value = toggle.checked ? '60' : '61';
            rate.disabled = !toggle.checked;
            if (toggle.checked) {
                rate.classList.remove('is-disabled');
            } else {
                rate.classList.add('is-disabled');
                if (!rate.value || rate.value === '0' || rate.value === '0.00') {
                    rate.value = '';
                }
            }
        }

        function initializeOvertimeUi() {
            var hidden = document.getElementById('<%= hdnOvertimeEligible.ClientID %>');
            var toggle = document.getElementById('chkOvertimeEligible');
            if (!hidden || !toggle) return;

            var normalized = (hidden.value || '').toString().toLowerCase();
            toggle.checked = normalized === '1' || normalized === '60' || normalized === 'yes' || normalized === 'y' || normalized === 'true';
            toggleOvertimeUi();
        }

        function blockNumbersAndSpecialChar(event) {
            var key = event.key;
            var regex = /^[a-zA-Z\s]$/;
            return regex.test(key);
        }

        function capitalizeFirstLetter(input) {
            if (input.value.length > 0) {
                input.value = input.value.charAt(0).toUpperCase() + input.value.slice(1);
            }
        }

        function checkNoLeadingSpace(input) {
            input.value = input.value.replace(/^\s+/, '');
        }

        function formatPhoneNumber(element) {
            var input = element.value.replace(/[^\d]/g, '');
            if (input.length > 10) {
                input = input.substring(0, 10);
            }
            element.value = input;
        }

        function setValidationState(element, isInvalid) {
            if (!element) return;
            element.classList.toggle('validation-error', !!isInvalid);
        }

        function getElementTrimmedValue(element) {
            if (!element) return '';
            return (element.value || '').toString().trim();
        }

        function validateRequiredSelect(element, label) {
            if (!element) return { valid: true, message: '' };

            var value = getElementTrimmedValue(element);
            var isValid = value !== '';
            setValidationState(element, !isValid);
            return {
                valid: isValid,
                message: isValid ? '' : (label + ' is required.')
            };
        }

        function validateRequiredInput(element, label) {
            if (!element) return { valid: true, message: '' };

            var value = getElementTrimmedValue(element);
            var isValid = value !== '';
            setValidationState(element, !isValid);
            return {
                valid: isValid,
                message: isValid ? '' : (label + ' is required.')
            };
        }

        function validatePatternField(element, label, pattern, message, isOptional) {
            if (!element) return { valid: true, message: '' };

            var value = getElementTrimmedValue(element);
            if (!value) {
                var validWhenEmpty = !!isOptional;
                setValidationState(element, !validWhenEmpty);
                return {
                    valid: validWhenEmpty,
                    message: validWhenEmpty ? '' : (label + ' is required.')
                };
            }

            var isValid = pattern.test(value);
            setValidationState(element, !isValid);
            return {
                valid: isValid,
                message: isValid ? '' : message
            };
        }

        function validateAddEmployeeClientForm() {
            var validations = [
                { element: document.getElementById('<%= txtEmployeeCode.ClientID %>'), type: 'required', name: 'Employee Code' },
                { element: document.getElementById('<%= txt_name.ClientID %>'), type: 'required', name: 'Username' },
                { element: document.getElementById('<%= txt_fullname.ClientID %>'), type: 'pattern', name: 'Employee Name', pattern: /^[A-Za-z ]+$/, message: 'Write name in correct format.' },
                { element: document.getElementById('<%= txtFirstName.ClientID %>'), type: 'pattern', name: 'First Name', pattern: /^[A-Za-z ]+$/, message: 'Write name in correct format.' },
                { element: document.getElementById('<%= txtMiddleName.ClientID %>'), type: 'pattern', name: 'Middle Name', pattern: /^[A-Za-z ]+$/, message: 'Write name in correct format.', optional: true },
                { element: document.getElementById('<%= txtLastName.ClientID %>'), type: 'pattern', name: 'Last Name', pattern: /^[A-Za-z ]+$/, message: 'Write name in correct format.' },
                { element: document.getElementById('<%= txtDisplayName.ClientID %>'), type: 'pattern', name: 'Display Name', pattern: /^[A-Za-z ]+$/, message: 'Write name in correct format.' },
                { element: document.getElementById('<%= txtGender.ClientID %>'), type: 'select', name: 'Gender' },
                { element: document.getElementById('<%= txtDOB.ClientID %>'), type: 'required', name: 'Date of Birth' },
                { element: document.getElementById('<%= txtMaritalStatus.ClientID %>'), type: 'select', name: 'Marital Status' },
                { element: document.getElementById('<%= txtBloodGroup.ClientID %>'), type: 'select', name: 'Blood Group' },
                { element: document.getElementById('<%= txtNationality.ClientID %>'), type: 'select', name: 'Nationality' },
                { element: document.getElementById('<%= txtAadhaarNumber.ClientID %>'), type: 'pattern', name: 'Aadhaar Number', pattern: /^\d{12}$/, message: 'Aadhaar Number must be 12 digits.' },
                { element: document.getElementById('<%= txtPanNumber.ClientID %>'), type: 'pattern', name: 'PAN Number', pattern: /^[A-Za-z]{5}\d{4}[A-Za-z]{1}$/, message: 'Enter a valid PAN Number.' },
                { element: document.getElementById('<%= txtPassportExpiryDate.ClientID %>'), type: 'required', name: 'Passport Expiry Date', optional: true },

                { element: document.getElementById('<%= txt_contact.ClientID %>'), type: 'pattern', name: 'Mobile Number', pattern: /^\d{10}$/, message: 'Mobile Number must be 10 digits.' },
                { element: document.getElementById('<%= txt_email.ClientID %>'), type: 'pattern', name: 'Official Email ID', pattern: /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/, message: 'Enter a valid Official Email ID.' },
                { element: document.getElementById('<%= txtPersonalEmail.ClientID %>'), type: 'pattern', name: 'Personal Email ID', pattern: /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/, message: 'Enter a valid Personal Email ID.', optional: true },

                { element: document.getElementById('<%= ddlexporintern.ClientID %>'), type: 'select', name: 'Employment Type' },
                { element: document.getElementById('<%= txtDateOfJoining.ClientID %>'), type: 'required', name: 'Date Of Joining' },
                { element: document.getElementById('<%= ddlprobationperiod.ClientID %>'), type: 'select', name: 'Probation Period' },
                { element: document.getElementById('<%= txtConfirmationDate.ClientID %>'), type: 'required', name: 'Confirmation Date', optional: true },
                { element: document.getElementById('<%= txtProbationEndDate.ClientID %>'), type: 'required', name: 'Probation End Date', optional: true },
                { element: document.getElementById('<%= txtEmployeeStatus.ClientID %>'), type: 'select', name: 'Employee Status' },
                { element: document.getElementById('<%= txtExitDate.ClientID %>'), type: 'required', name: 'Exit Date', optional: true },

                { element: document.getElementById('<%= ddlcompany.ClientID %>'), type: 'select', name: 'Company' },
                { element: document.getElementById('<%= txtdept.ClientID %>'), type: 'required', name: 'Department' },
                { element: document.getElementById('<%= txtbranch.ClientID %>'), type: 'required', name: 'Branch Office' },
                { element: document.getElementById('<%= txtLocation.ClientID %>'), type: 'required', name: 'Location' },
                { element: document.getElementById('<%= ddldesign.ClientID %>'), type: 'select', name: 'Designation' },
                { element: document.getElementById('<%= ddl_reportingmanager.ClientID %>'), type: 'select', name: 'Reporting Manager' },
                { element: document.getElementById('<%= txtFunctionalManager.ClientID %>'), type: 'required', name: 'Functional Manager' },
                { element: document.getElementById('<%= txtHod.ClientID %>'), type: 'required', name: 'HOD' },
                { element: document.getElementById('<%= txtEmployeeLevel.ClientID %>'), type: 'select', name: 'Employee Level', optional: true },

                { element: document.getElementById('<%= txtAttendanceType.ClientID %>'), type: 'select', name: 'Attendance Type' },
                { element: document.getElementById('<%= txtWeeklyOff.ClientID %>'), type: 'select', name: 'Weekly Off' },
                { element: document.getElementById('<%= txtWorkingHours.ClientID %>'), type: 'pattern', name: 'Working Hours', pattern: /^([0-9]|[01][0-9]|2[0-3]):[0-5][0-9]$/, message: 'Working Hours must be in HH:mm format.' },
                { element: document.getElementById('<%= txtAttendancePolicy.ClientID %>'), type: 'select', name: 'Attendance Policy' },
                { element: document.getElementById('<%= txtPunchingDeviceId.ClientID %>'), type: 'required', name: 'Punching Device ID' },
                { element: document.getElementById('<%= txtBiometricId.ClientID %>'), type: 'required', name: 'Biometric ID' },
                { element: document.getElementById('<%= txtWorkLocation.ClientID %>'), type: 'select', name: 'Work Location' }
            ];

            var firstInvalidElement = null;
            var firstMessage = '';

            validations.forEach(function (item) {
                var result;
                if (item.type === 'select') {
                    if (item.optional && !getElementTrimmedValue(item.element)) {
                        setValidationState(item.element, false);
                        result = { valid: true, message: '' };
                    } else {
                        result = validateRequiredSelect(item.element, item.name);
                    }
                } else if (item.type === 'pattern') {
                    result = validatePatternField(item.element, item.name, item.pattern, item.message, !!item.optional);
                } else {
                    if (item.optional && !getElementTrimmedValue(item.element)) {
                        setValidationState(item.element, false);
                        result = { valid: true, message: '' };
                    } else {
                        result = validateRequiredInput(item.element, item.name);
                    }
                }

                if (!result.valid && !firstInvalidElement) {
                    firstInvalidElement = item.element;
                    firstMessage = result.message;
                }
            });

            var overtimeToggle = document.getElementById('chkOvertimeEligible');
            var overtimeRate = document.getElementById('<%= txtOvertimeRate.ClientID %>');
            if (overtimeToggle && overtimeToggle.checked) {
                var overtimeResult = validateRequiredInput(overtimeRate, 'Overtime Rate');
                if (!overtimeResult.valid && !firstInvalidElement) {
                    firstInvalidElement = overtimeRate;
                    firstMessage = overtimeResult.message;
                }
            } else {
                setValidationState(overtimeRate, false);
            }

            if (firstInvalidElement) {
                firstInvalidElement.focus();
                showUserSavedMessage('Failed', firstMessage);
                return false;
            }

            return true;
        }

        function validateOfficialContactClientForm() {
            clearOfficialContactInlineErrors();
            var validations = [
                { element: document.getElementById('<%= txt_contact.ClientID %>'), type: 'pattern', name: 'Mobile Number', pattern: /^\d{10}$/, message: 'Mobile Number must be 10 digits.' },
                { element: document.getElementById('<%= txt_email.ClientID %>'), type: 'pattern', name: 'Official Email ID', pattern: /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/, message: 'Enter a valid Official Email ID.' }
            ];

            var firstInvalidElement = null;
            var firstMessage = '';

            validations.forEach(function (item) {
                var result = validatePatternField(item.element, item.name, item.pattern, item.message, false);
                if (!result.valid && !firstInvalidElement) {
                    firstInvalidElement = item.element;
                    firstMessage = result.message;
                }
            });

            if (firstInvalidElement) {
                firstInvalidElement.focus();
                showUserSavedMessage('Failed', firstMessage);
                return false;
            }

            return true;
        }

        function setOfficialContactInlineError(fieldName, message) {
            var isEmail = (fieldName || '').toLowerCase() === 'email';
            var element = document.getElementById(isEmail ? '<%= txt_email.ClientID %>' : '<%= txt_contact.ClientID %>');
            var messageElement = document.getElementById(isEmail ? 'officialEmailInlineError' : 'officialMobileInlineError');
            var hasMessage = !!message;

            if (element) {
                setValidationState(element, hasMessage);
                if (hasMessage) {
                    element.focus();
                }
            }

            if (messageElement) {
                messageElement.textContent = message || '';
                messageElement.style.display = hasMessage ? 'block' : 'none';
            }
        }

        function checkOfficialContactDuplicateField(fieldName, element) {
            if (!element) {
                return;
            }

            var value = getElementTrimmedValue(element);
            var isEmail = fieldName === 'Email';
            var formatMessage = '';

            if (!value) {
                setOfficialContactInlineError(isEmail ? 'email' : 'mobile', '');
                setValidationState(element, false);
                return;
            }

            if (isEmail && !/^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/.test(value)) {
                setOfficialContactInlineError('email', 'Enter a valid Official Email ID.');
                return;
            }

            if (!isEmail && !/^\d{10}$/.test(value)) {
                setOfficialContactInlineError('mobile', 'Mobile Number must be 10 digits.');
                return;
            }

            fetch('<%= ResolveUrl("~/View/Modules/AddEmployee.aspx") %>'
                + '?action=checkOfficialContactDuplicate'
                + '&userId=' + encodeURIComponent('<%= Convert.ToString(Request.QueryString["user_id"] ?? "0") %>')
                + '&fieldName=' + encodeURIComponent(fieldName)
                + '&fieldValue=' + encodeURIComponent(value), {
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
                    var message = response && response.IsDuplicate ? response.Message : '';
                    setOfficialContactInlineError(isEmail ? 'email' : 'mobile', message || '');
                    if (!message) {
                        setValidationState(element, false);
                    }
                })
                .catch(function () {
                });
        }

        function clearOfficialContactInlineErrors() {
            var email = document.getElementById('<%= txt_email.ClientID %>');
            var mobile = document.getElementById('<%= txt_contact.ClientID %>');
            var emailMessage = document.getElementById('officialEmailInlineError');
            var mobileMessage = document.getElementById('officialMobileInlineError');

            setValidationState(email, false);
            setValidationState(mobile, false);

            if (emailMessage) {
                emailMessage.textContent = '';
                emailMessage.style.display = 'none';
            }

            if (mobileMessage) {
                mobileMessage.textContent = '';
                mobileMessage.style.display = 'none';
            }
        }

        document.addEventListener('DOMContentLoaded', function () {
            var email = document.getElementById('<%= txt_email.ClientID %>');
            var mobile = document.getElementById('<%= txt_contact.ClientID %>');
            var duplicateCheckTimers = {};

            if (email) {
                email.addEventListener('input', function () {
                    setValidationState(email, false);
                    var emailMessage = document.getElementById('officialEmailInlineError');
                    if (emailMessage) {
                        emailMessage.textContent = '';
                        emailMessage.style.display = 'none';
                    }

                    window.clearTimeout(duplicateCheckTimers.Email);
                    duplicateCheckTimers.Email = window.setTimeout(function () {
                        checkOfficialContactDuplicateField('Email', email);
                    }, 450);
                });

                email.addEventListener('blur', function () {
                    window.clearTimeout(duplicateCheckTimers.Email);
                    checkOfficialContactDuplicateField('Email', email);
                });
            }

            if (mobile) {
                mobile.addEventListener('keydown', function (event) {
                    var allowedKeys = ['Backspace', 'Delete', 'Tab', 'ArrowLeft', 'ArrowRight', 'Home', 'End'];
                    if (allowedKeys.indexOf(event.key) >= 0 || event.ctrlKey || event.metaKey) {
                        return;
                    }

                    if (!/^\d$/.test(event.key) || mobile.value.length >= 10) {
                        event.preventDefault();
                    }
                });

                mobile.addEventListener('paste', function (event) {
                    event.preventDefault();
                    var pastedText = (event.clipboardData || window.clipboardData).getData('text') || '';
                    mobile.value = pastedText.replace(/\D/g, '').slice(0, 10);
                    checkOfficialContactDuplicateField('Mobile', mobile);
                });

                mobile.addEventListener('input', function () {
                    var digitsOnly = mobile.value.replace(/\D/g, '').slice(0, 10);
                    if (mobile.value !== digitsOnly) {
                        mobile.value = digitsOnly;
                    }

                    setValidationState(mobile, false);
                    var mobileMessage = document.getElementById('officialMobileInlineError');
                    if (mobileMessage) {
                        mobileMessage.textContent = '';
                        mobileMessage.style.display = 'none';
                    }

                    window.clearTimeout(duplicateCheckTimers.Mobile);
                    duplicateCheckTimers.Mobile = window.setTimeout(function () {
                        checkOfficialContactDuplicateField('Mobile', mobile);
                    }, 450);
                });

                mobile.addEventListener('blur', function () {
                    window.clearTimeout(duplicateCheckTimers.Mobile);
                    checkOfficialContactDuplicateField('Mobile', mobile);
                });
            }
        });

        function validateAssetClientForm() {
            var validations = [
                { element: document.getElementById('<%= txtAssetType.ClientID %>'), type: 'required', name: 'Asset Type' },
                { element: document.getElementById('<%= txtAssetNumber.ClientID %>'), type: 'required', name: 'Asset Number' },
                { element: document.getElementById('<%= txtAssetName.ClientID %>'), type: 'required', name: 'Asset Name' },
                { element: document.getElementById('<%= txtAssignedDate.ClientID %>'), type: 'required', name: 'Assigned Date' },
                { element: document.getElementById('<%= ddlAssetCondition.ClientID %>'), type: 'select', name: 'Asset Condition' },
                { element: document.getElementById('<%= ddlAssetStatus.ClientID %>'), type: 'select', name: 'Asset Status' }
            ];

            var firstInvalidElement = null;
            var firstMessage = '';

            validations.forEach(function (item) {
                var result = item.type === 'select'
                    ? validateRequiredSelect(item.element, item.name)
                    : validateRequiredInput(item.element, item.name);

                if (!result.valid && !firstInvalidElement) {
                    firstInvalidElement = item.element;
                    firstMessage = result.message;
                }
            });

            setValidationState(document.getElementById('<%= txtReturnDate.ClientID %>'), false);

            if (firstInvalidElement) {
                firstInvalidElement.focus();
                showUserSavedMessage('Failed', firstMessage);
                return false;
            }

            return true;
        }

        function bindLiveValidation(fieldConfigs) {
            fieldConfigs.forEach(function (item) {
                if (!item.element) return;

                var handler = function () {
                    if (item.type === 'select') {
                        if (item.optional && !getElementTrimmedValue(item.element)) {
                            setValidationState(item.element, false);
                            return;
                        }

                        validateRequiredSelect(item.element, item.name);
                        return;
                    }

                    if (item.type === 'pattern') {
                        validatePatternField(item.element, item.name, item.pattern, item.message, !!item.optional);
                        return;
                    }

                    if (item.optional && !getElementTrimmedValue(item.element)) {
                        setValidationState(item.element, false);
                        return;
                    }

                    validateRequiredInput(item.element, item.name);
                };

                item.element.addEventListener('input', handler);
                item.element.addEventListener('change', handler);
                item.element.addEventListener('blur', handler);
            });
        }

        function bindGenericSelectionValidation() {
            document.querySelectorAll('.field-block select, .field-block input[type="date"]').forEach(function (element) {
                var handler = function () {
                    if ((element.value || '').toString().trim()) {
                        setValidationState(element, false);
                    }
                };

                element.addEventListener('change', handler);
                element.addEventListener('blur', handler);
            });
        }

        function updateProfileSummary() {
            var get = function (id) {
                var el = document.getElementById(id);
                if (!el) return '';
                if (el.tagName === 'SELECT') {
                    return el.selectedIndex >= 0 ? el.options[el.selectedIndex].text.trim() : '';
                }
                return (el.value || '').trim();
            };

            var setText = function (id, text) {
                var el = document.getElementById(id);
                if (el) el.textContent = text || '-';
            };

            setText('profileEmployeeCode', get('<%= txtEmployeeCode.ClientID %>'));
            setText('profileEmployeeName', get('<%= txt_fullname.ClientID %>'));
            setText('profileEmail', get('<%= txt_email.ClientID %>'));
            setText('profileMobile', get('<%= txt_contact.ClientID %>'));
            setText('profileJoining', get('<%= txtDateOfJoining.ClientID %>'));
            setText('profileDesignation', get('<%= ddldesign.ClientID %>'));
            setText('profileDepartment', [get('<%= txtdept.ClientID %>'), get('<%= txtbranch.ClientID %>')].filter(Boolean).join(' | '));
            setText('profileReporting', get('<%= ddl_reportingmanager.ClientID %>'));
        }

        function openEmployeePhotoModal() {
            var modal = document.getElementById('employeePhotoModal');
            if (!modal) return;
            modal.classList.add('is-open');
            document.body.style.overflow = 'hidden';
        }

        function closeEmployeePhotoModal() {
            var modal = document.getElementById('employeePhotoModal');
            if (!modal) return;
            modal.classList.remove('is-open');
            document.body.style.overflow = '';
        }

        function closeEmployeePhotoModalOnBackdrop(event) {
            if (event.target && event.target.id === 'employeePhotoModal') {
                closeEmployeePhotoModal();
            }
        }

        function openCertificationFileModal(link) {
            if (!link) return false;

            var fileUrl = link.getAttribute('data-file-url') || link.getAttribute('href') || '';
            var fileType = (link.getAttribute('data-file-type') || 'image').toLowerCase();
            var fileTitle = link.getAttribute('data-file-title') || 'Professional Certificate';
            var modal = document.getElementById('certificationFileModal');
            var image = document.getElementById('certificationPreviewImage');
            var frame = document.getElementById('certificationPreviewFrame');
            var title = document.getElementById('certificationFileModalTitle');

            if (!modal || !fileUrl || !image || !frame) {
                return false;
            }

            if (title) {
                title.textContent = fileTitle;
            }

            image.classList.add('is-hidden');
            frame.classList.add('is-hidden');
            image.removeAttribute('src');
            frame.removeAttribute('src');

            if (fileType === 'pdf') {
                frame.src = fileUrl;
                frame.classList.remove('is-hidden');
            } else {
                image.src = fileUrl;
                image.classList.remove('is-hidden');
            }

            modal.classList.add('is-open');
            document.body.style.overflow = 'hidden';
            return false;
        }

        function closeCertificationFileModal() {
            var modal = document.getElementById('certificationFileModal');
            var image = document.getElementById('certificationPreviewImage');
            var frame = document.getElementById('certificationPreviewFrame');
            var title = document.getElementById('certificationFileModalTitle');

            if (image) {
                image.removeAttribute('src');
                image.classList.add('is-hidden');
            }

            if (frame) {
                frame.removeAttribute('src');
                frame.classList.add('is-hidden');
            }

            if (modal) {
                modal.classList.remove('is-open');
            }

            if (title) {
                title.textContent = 'Professional Certificate';
            }

            document.body.style.overflow = '';
        }

        function closeCertificationFileModalOnBackdrop(event) {
            if (event.target && event.target.id === 'certificationFileModal') {
                closeCertificationFileModal();
            }
        }

        $(document).ready(function () {
            updateProfileSummary();
            initializeOvertimeUi();
            $('input, select').on('input change', updateProfileSummary);

            bindLiveValidation([
                { element: document.getElementById('<%= txtEmployeeCode.ClientID %>'), type: 'required', name: 'Employee Code' },
                { element: document.getElementById('<%= txt_name.ClientID %>'), type: 'required', name: 'Username' },
                { element: document.getElementById('<%= txt_fullname.ClientID %>'), type: 'pattern', name: 'Employee Name', pattern: /^[A-Za-z ]+$/, message: 'Write name in correct format.' },
                { element: document.getElementById('<%= txtFirstName.ClientID %>'), type: 'pattern', name: 'First Name', pattern: /^[A-Za-z ]+$/, message: 'Write name in correct format.' },
                { element: document.getElementById('<%= txtMiddleName.ClientID %>'), type: 'pattern', name: 'Middle Name', pattern: /^[A-Za-z ]+$/, message: 'Write name in correct format.', optional: true },
                { element: document.getElementById('<%= txtLastName.ClientID %>'), type: 'pattern', name: 'Last Name', pattern: /^[A-Za-z ]+$/, message: 'Write name in correct format.' },
                { element: document.getElementById('<%= txtDisplayName.ClientID %>'), type: 'pattern', name: 'Display Name', pattern: /^[A-Za-z ]+$/, message: 'Write name in correct format.' },
                { element: document.getElementById('<%= txtGender.ClientID %>'), type: 'select', name: 'Gender' },
                { element: document.getElementById('<%= txtDOB.ClientID %>'), type: 'required', name: 'Date of Birth' },
                { element: document.getElementById('<%= txtMaritalStatus.ClientID %>'), type: 'select', name: 'Marital Status' },
                { element: document.getElementById('<%= txtBloodGroup.ClientID %>'), type: 'select', name: 'Blood Group' },
                { element: document.getElementById('<%= txtNationality.ClientID %>'), type: 'select', name: 'Nationality' },
                { element: document.getElementById('<%= txtAadhaarNumber.ClientID %>'), type: 'pattern', name: 'Aadhaar Number', pattern: /^\d{12}$/, message: 'Aadhaar Number must be 12 digits.' },
                { element: document.getElementById('<%= txtPanNumber.ClientID %>'), type: 'pattern', name: 'PAN Number', pattern: /^[A-Za-z]{5}\d{4}[A-Za-z]{1}$/, message: 'Enter a valid PAN Number.' },
                { element: document.getElementById('<%= txtPassportExpiryDate.ClientID %>'), type: 'required', name: 'Passport Expiry Date', optional: true },
                { element: document.getElementById('<%= txt_contact.ClientID %>'), type: 'pattern', name: 'Mobile Number', pattern: /^\d{10}$/, message: 'Mobile Number must be 10 digits.' },
                { element: document.getElementById('<%= txt_email.ClientID %>'), type: 'pattern', name: 'Official Email ID', pattern: /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/, message: 'Enter a valid Official Email ID.' },
                { element: document.getElementById('<%= txtPersonalEmail.ClientID %>'), type: 'pattern', name: 'Personal Email ID', pattern: /^[^\s@]+@[^\s@]+\.[^\s@]{2,}$/, message: 'Enter a valid Personal Email ID.', optional: true },
                { element: document.getElementById('<%= ddlexporintern.ClientID %>'), type: 'select', name: 'Employment Type' },
                { element: document.getElementById('<%= txtDateOfJoining.ClientID %>'), type: 'required', name: 'Date Of Joining' },
                { element: document.getElementById('<%= ddlprobationperiod.ClientID %>'), type: 'select', name: 'Probation Period' },
                { element: document.getElementById('<%= txtConfirmationDate.ClientID %>'), type: 'required', name: 'Confirmation Date', optional: true },
                { element: document.getElementById('<%= txtProbationEndDate.ClientID %>'), type: 'required', name: 'Probation End Date', optional: true },
                { element: document.getElementById('<%= txtEmployeeStatus.ClientID %>'), type: 'select', name: 'Employee Status' },
                { element: document.getElementById('<%= txtExitDate.ClientID %>'), type: 'required', name: 'Exit Date', optional: true },
                { element: document.getElementById('<%= ddlcompany.ClientID %>'), type: 'select', name: 'Company' },
                { element: document.getElementById('<%= txtdept.ClientID %>'), type: 'required', name: 'Department' },
                { element: document.getElementById('<%= txtbranch.ClientID %>'), type: 'required', name: 'Branch Office' },
                { element: document.getElementById('<%= txtLocation.ClientID %>'), type: 'required', name: 'Location' },
                { element: document.getElementById('<%= ddldesign.ClientID %>'), type: 'select', name: 'Designation' },
                { element: document.getElementById('<%= ddl_reportingmanager.ClientID %>'), type: 'select', name: 'Reporting Manager' },
                { element: document.getElementById('<%= txtFunctionalManager.ClientID %>'), type: 'required', name: 'Functional Manager' },
                { element: document.getElementById('<%= txtHod.ClientID %>'), type: 'required', name: 'HOD' },
                { element: document.getElementById('<%= txtEmployeeLevel.ClientID %>'), type: 'select', name: 'Employee Level', optional: true },
                { element: document.getElementById('<%= txtAttendanceType.ClientID %>'), type: 'select', name: 'Attendance Type' },
                { element: document.getElementById('<%= txtWeeklyOff.ClientID %>'), type: 'select', name: 'Weekly Off' },
                { element: document.getElementById('<%= txtWorkingHours.ClientID %>'), type: 'pattern', name: 'Working Hours', pattern: /^([0-9]|[01][0-9]|2[0-3]):[0-5][0-9]$/, message: 'Working Hours must be in HH:mm format.' },
                { element: document.getElementById('<%= txtAttendancePolicy.ClientID %>'), type: 'select', name: 'Attendance Policy' },
                { element: document.getElementById('<%= txtPunchingDeviceId.ClientID %>'), type: 'required', name: 'Punching Device ID' },
                { element: document.getElementById('<%= txtBiometricId.ClientID %>'), type: 'required', name: 'Biometric ID' },
                { element: document.getElementById('<%= txtWorkLocation.ClientID %>'), type: 'select', name: 'Work Location' },
                { element: document.getElementById('<%= txtAssetType.ClientID %>'), type: 'required', name: 'Asset Type' },
                { element: document.getElementById('<%= txtAssetNumber.ClientID %>'), type: 'required', name: 'Asset Number' },
                { element: document.getElementById('<%= txtAssetName.ClientID %>'), type: 'required', name: 'Asset Name' },
                { element: document.getElementById('<%= txtAssignedDate.ClientID %>'), type: 'required', name: 'Assigned Date' },
                { element: document.getElementById('<%= ddlAssetCondition.ClientID %>'), type: 'select', name: 'Asset Condition' },
                { element: document.getElementById('<%= ddlAssetStatus.ClientID %>'), type: 'select', name: 'Asset Status' }
            ]);

            var overtimeRateElement = document.getElementById('<%= txtOvertimeRate.ClientID %>');
            if (overtimeRateElement) {
                ['input', 'change', 'blur'].forEach(function (eventName) {
                    overtimeRateElement.addEventListener(eventName, function () {
                        var overtimeToggle = document.getElementById('chkOvertimeEligible');
                        if (overtimeToggle && overtimeToggle.checked) {
                            validateRequiredInput(overtimeRateElement, 'Overtime Rate');
                        } else {
                            setValidationState(overtimeRateElement, false);
                        }
                    });
                });
            }

            bindGenericSelectionValidation();
        });
    </script>
</asp:Content>
