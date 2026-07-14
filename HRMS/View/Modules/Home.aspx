<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="HRMS.View.Modules.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <style>
        .hb-wrap {
            padding: 24px;
            font-family: 'Segoe UI', Arial, sans-serif;
        }

        .hb-welcome {
            font-size: 22px;
            font-weight: 600;
            color: #222;
            margin-bottom: 4px;
        }

        .hb-subtext {
            color: #7a7a7a;
            font-size: 14px;
            margin-bottom: 22px;
        }

        .hb-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 20px;
        }

        @media (max-width: 992px) {
            .hb-grid {
                grid-template-columns: repeat(2, 1fr);
            }
        }

        @media (max-width: 640px) {
            .hb-grid {
                grid-template-columns: 1fr;
            }
        }

        .hb-slideshow {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 16px;
            box-shadow: 0 10px 40px rgba(102, 126, 234, 0.3);
            overflow: hidden;
            position: relative;
            height: 220px;
            margin-bottom: 24px;
        }

        .hb-slideshow-content {
            position: relative;
            height: 100%;
            overflow: hidden;
        }

        .hb-slide {
            position: absolute;
            width: 100%;
            height: 100%;
            opacity: 0;
            transition: all 1s ease-in-out;
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 24px 32px;
        }

        .hb-slide.active {
            opacity: 1;
        }

        .hb-slide-text {
            z-index: 10;
            color: white;
            max-width: 50%;
        }

        .hb-slide-title {
            font-size: 28px;
            font-weight: 800;
            margin-bottom: 8px;
            text-shadow: 0 2px 10px rgba(0,0,0,0.2);
        }

        .hb-slide-meta {
            font-size: 14px;
            opacity: 0.9;
            margin-bottom: 12px;
        }

        .hb-slide-description {
            font-size: 16px;
            line-height: 1.6;
            opacity: 0.95;
        }

        .hb-slide-badge {
            display: inline-block;
            padding: 6px 16px;
            border-radius: 24px;
            font-size: 12px;
            font-weight: 700;
            margin-bottom: 12px;
            background: rgba(255,255,255,0.25);
            backdrop-filter: blur(10px);
        }

        .hb-slide-image {
            z-index: 5;
            width: 300px;
            height: 200px;
            object-fit: cover;
            border-radius: 16px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.3);
            animation: float 3s ease-in-out infinite;
        }

        @keyframes float {
            0%, 100% { transform: translateY(0px); }
            50% { transform: translateY(-10px); }
        }

        .hb-slideshow-controls {
            position: absolute;
            bottom: 16px;
            left: 50%;
            transform: translateX(-50%);
            display: flex;
            gap: 10px;
            z-index: 20;
        }

        .hb-slideshow-dot {
            width: 14px;
            height: 14px;
            border-radius: 50%;
            background: rgba(255,255,255,0.4);
            cursor: pointer;
            transition: all 0.4s ease;
            border: 2px solid transparent;
        }

        .hb-slideshow-dot:hover {
            background: rgba(255,255,255,0.6);
        }

        .hb-slideshow-dot.active {
            background: white;
            transform: scale(1.3);
            border-color: rgba(255,255,255,0.5);
        }

        .hb-card {
            background: #ffffff;
            border-radius: 14px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.06);
            border: 1px solid #eef0f3;
            overflow: hidden;
            display: flex;
            flex-direction: column;
        }

        .hb-card-head {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 16px 18px;
            border-bottom: 1px solid #f0f2f5;
        }

        .hb-icon {
            width: 38px;
            height: 38px;
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
        }

        .hb-icon svg {
            width: 20px;
            height: 20px;
        }

        .hb-icon.news { background: #e8f0fe; }
        .hb-icon.birthday { background: #fdeaea; }
        .hb-icon.events { background: #e9f9ef; }
        .hb-icon.holiday { background: #fff4e0; }

        .hb-card-title {
            font-size: 15px;
            font-weight: 600;
            color: #222;
        }

        .hb-card-sub {
            font-size: 12px;
            color: #9a9a9a;
        }

        .hb-card-body {
            padding: 10px 18px 16px 18px;
            flex: 1;
        }

        .hb-item {
            display: flex;
            align-items: flex-start;
            gap: 10px;
            padding: 10px 0;
            border-bottom: 1px dashed #eef0f3;
        }

        .hb-item:last-child {
            border-bottom: none;
        }

        .hb-avatar {
            width: 34px;
            height: 34px;
            border-radius: 50%;
            background: #2f6fed;
            color: #fff;
            font-size: 13px;
            font-weight: 600;
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
        }

        .hb-item-title {
            font-size: 13.5px;
            font-weight: 600;
            color: #333;
            margin-bottom: 2px;
        }

        .hb-item-meta {
            font-size: 12px;
            color: #9a9a9a;
        }

        .hb-badge {
            margin-left: auto;
            font-size: 11px;
            font-weight: 600;
            padding: 3px 9px;
            border-radius: 20px;
            white-space: nowrap;
            height: fit-content;
        }

        .hb-badge.today { background: #fdeaea; color: #e0455a; }
        .hb-badge.upcoming { background: #e8f0fe; color: #2f6fed; }
        .hb-badge.date { background: #f0f2f5; color: #666; }

        .hb-empty {
            font-size: 13px;
            color: #aaa;
            padding: 20px 0;
            text-align: center;
        }

        .hb-viewall {
            display: block;
            text-align: center;
            padding: 10px;
            font-size: 13px;
            font-weight: 600;
            color: #2f6fed;
            border-top: 1px solid #f0f2f5;
            text-decoration: none;
            cursor: pointer;
        }

        .hb-viewall:hover {
            background: #f7f9fc;
            color: #1d54c4;
        }

        .hb-stats-row {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 20px;
            margin-bottom: 20px;
        }

        @media (max-width: 992px) {
            .hb-stats-row { grid-template-columns: repeat(2, 1fr); }
        }
        @media (max-width: 640px) {
            .hb-stats-row { grid-template-columns: 1fr; }
        }

        .hb-stat {
            background: #ffffff;
            border-radius: 14px;
            border: 1px solid #eef0f3;
            box-shadow: 0 2px 10px rgba(0,0,0,0.05);
            padding: 16px 18px;
            display: flex;
            align-items: center;
            gap: 12px;
        }

        .hb-stat-num {
            font-size: 20px;
            font-weight: 700;
            color: #222;
        }

        .hb-stat-label {
            font-size: 12px;
            color: #9a9a9a;
        }

        .hb-extra {
            display: none;
        }

        .hb-extra-count {
            font-size: 11px;
            opacity: 0.75;
        }

        .hb-add-btn {
            display: inline-flex;
            align-items: center;
            gap: 5px;
            margin-left: auto;
            font-size: 12px;
            font-weight: 600;
            color: #2f6fed;
            background: #e8f0fe;
            border: none;
            border-radius: 8px;
            padding: 5px 11px;
            text-decoration: none;
            white-space: nowrap;
            cursor: pointer;
            flex-shrink: 0;
        }

        .hb-add-btn:hover {
            background: #d2e3fc;
            color: #1d54c4;
        }

        /* Modal */
        .hb-modal-overlay {
            display: none;
            position: fixed;
            inset: 0;
            background: rgba(0,0,0,0.35);
            z-index: 1000;
            align-items: center;
            justify-content: center;
        }

        .hb-modal-overlay.active {
            display: flex;
        }

        .hb-modal {
            background: #fff;
            border-radius: 16px;
            box-shadow: 0 8px 40px rgba(0,0,0,0.14);
            width: 100%;
            max-width: 460px;
            margin: 16px;
            animation: hb-modal-in 0.18s ease;
        }

        @keyframes hb-modal-in {
            from { opacity: 0; transform: translateY(14px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        .hb-modal-header {
            display: flex;
            align-items: center;
            gap: 10px;
            padding: 18px 20px 14px;
            border-bottom: 1px solid #f0f2f5;
        }

        .hb-modal-title {
            font-size: 15px;
            font-weight: 700;
            color: #222;
            flex: 1;
        }

        .hb-modal-close {
            width: 28px;
            height: 28px;
            border: none;
            background: #f0f2f5;
            border-radius: 50%;
            cursor: pointer;
            display: flex;
            align-items: center;
            justify-content: center;
            color: #555;
            font-size: 16px;
            line-height: 1;
            padding: 0;
        }

        .hb-modal-close:hover { background: #e0e4ea; }

        .hb-modal-body {
            padding: 18px 20px;
            display: flex;
            flex-direction: column;
            gap: 14px;
        }

        .hb-field {
            display: flex;
            flex-direction: column;
            gap: 5px;
        }

        .hb-field label {
            font-size: 12px;
            font-weight: 600;
            color: #555;
        }

        .hb-field input,
        .hb-field select,
        .hb-field textarea {
            border: 1px solid #dde1e8;
            border-radius: 8px;
            padding: 8px 11px;
            font-size: 13.5px;
            color: #222;
            font-family: inherit;
            outline: none;
            transition: border-color 0.15s;
            background: #fff;
        }

        .hb-field input:focus,
        .hb-field select:focus,
        .hb-field textarea:focus {
            border-color: #2f6fed;
            box-shadow: 0 0 0 3px rgba(47,111,237,0.1);
        }

        .hb-field textarea {
            resize: vertical;
            min-height: 72px;
        }

        .hb-field-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 12px;
        }

        .hb-modal-footer {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            padding: 12px 20px 18px;
        }

        .hb-btn {
            padding: 8px 20px;
            border-radius: 8px;
            font-size: 13.5px;
            font-weight: 600;
            cursor: pointer;
            border: none;
            font-family: inherit;
        }

        .hb-btn-ghost {
            background: #f0f2f5;
            color: #555;
        }

        .hb-btn-ghost:hover { background: #e0e4ea; }

        .hb-btn-primary {
            background: #2f6fed;
            color: #fff;
        }

        .hb-btn-primary:hover { background: #1d54c4; }
    </style>

    <div class="hb-wrap">

        <div class="hb-welcome">Welcome back, <asp:Literal ID="litUserName" runat="server" Text="meera123" />!</div>
        <div class="hb-subtext">Here's what's happening today.</div>

        <!-- Slideshow Banner -->
        <div class="hb-slideshow">
            <div class="hb-slideshow-content">
                <div class="hb-slide active" data-slide="0" style="background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);">
                    <div class="hb-slide-text">
                        <span class="hb-slide-badge">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="vertical-align: middle; margin-right: 4px;">
                                <path d="M8 2v4M16 2v4M3 10h18M5 4h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2z"/>
                            </svg>
                            Event
                        </span>
                        <div class="hb-slide-title">Team Building Activity</div>
                        <div class="hb-slide-meta">18 July 2026 • 2:00 PM</div>
                        <div class="hb-slide-description">Join us for a fun team-building activity to strengthen collaboration and boost morale!</div>
                    </div>
                    <img class="hb-slide-image" src="https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=300&h=200&fit=crop" alt="Team Building">
                </div>
                <div class="hb-slide" data-slide="1" style="background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);">
                    <div class="hb-slide-text">
                        <span class="hb-slide-badge">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="vertical-align: middle; margin-right: 4px;">
                                <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                                <polyline points="14,2 14,8 20,8"/>
                                <line x1="16" y1="13" x2="8" y2="13"/>
                                <line x1="16" y1="17" x2="8" y2="17"/>
                                <polyline points="10,9 9,9 8,9"/>
                            </svg>
                            News
                        </span>
                        <div class="hb-slide-title">New Leave Policy</div>
                        <div class="hb-slide-meta">Posted 2 days ago</div>
                        <div class="hb-slide-description">New leave policy effective August 2026. Please review the updated guidelines.</div>
                    </div>
                    <img class="hb-slide-image" src="https://images.unsplash.com/photo-1454165804606-c3d57bc86b40?w=300&h=200&fit=crop" alt="Policy Update">
                </div>
                <div class="hb-slide" data-slide="2" style="background: linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%);">
                    <div class="hb-slide-text">
                        <span class="hb-slide-badge">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="vertical-align: middle; margin-right: 4px;">
                                <path d="M12 6c1 0 1.5-1 1-2s-1-1-1-2c-.5 1-1 1-1 2s0 2 1 2zM4 22v-8a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8M2 22h20M4 14c1-1 2 1 3 0s2-1 3 0 2 1 3 0 2-1 3 0 2 1 3 0"/>
                            </svg>
                            Birthday
                        </span>
                        <div class="hb-slide-title">Happy Birthday Riya!</div>
                        <div class="hb-slide-meta">Today • HR Department</div>
                        <div class="hb-slide-description">Join us in wishing Riya a very happy birthday! 🎉</div>
                    </div>
                    <img class="hb-slide-image" src="https://images.unsplash.com/photo-1578985545062-69928b1d9587?w=300&h=200&fit=crop" alt="Birthday">
                </div>
                <div class="hb-slide" data-slide="3" style="background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);">
                    <div class="hb-slide-text">
                        <span class="hb-slide-badge">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="vertical-align: middle; margin-right: 4px;">
                                <path d="M23 6a2 2 0 0 1-2 2H7l-4 4V4a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/>
                            </svg>
                            Town Hall
                        </span>
                        <div class="hb-slide-title">Quarterly Town Hall</div>
                        <div class="hb-slide-meta">22 July 2026 • 4:00 PM</div>
                        <div class="hb-slide-description">Attend our quarterly town hall to discuss company achievements and future goals.</div>
                    </div>
                    <img class="hb-slide-image" src="https://images.unsplash.com/photo-1556761175-5973dc0f32e7?w=300&h=200&fit=crop" alt="Town Hall">
                </div>
                <div class="hb-slide" data-slide="4" style="background: linear-gradient(135deg, #fa709a 0%, #fee140 100%);">
                    <div class="hb-slide-text">
                        <span class="hb-slide-badge">
                            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="vertical-align: middle; margin-right: 4px;">
                                <rect x="2" y="3" width="20" height="14" rx="2" ry="2"/>
                                <line x1="8" y1="21" x2="16" y2="21"/>
                                <line x1="12" y1="17" x2="12" y2="21"/>
                            </svg>
                            Maintenance
                        </span>
                        <div class="hb-slide-title">Server Maintenance</div>
                        <div class="hb-slide-meta">Posted 4 days ago</div>
                        <div class="hb-slide-description">Scheduled server maintenance this weekend. Expect brief service interruptions.</div>
                    </div>
                    <img class="hb-slide-image" src="https://images.unsplash.com/photo-1558494949-ef010cbdcc31?w=300&h=200&fit=crop" alt="Server Maintenance">
                </div>
                <div class="hb-slideshow-controls">
                    <div class="hb-slideshow-dot active" data-slide="0"></div>
                    <div class="hb-slideshow-dot" data-slide="1"></div>
                    <div class="hb-slideshow-dot" data-slide="2"></div>
                    <div class="hb-slideshow-dot" data-slide="3"></div>
                    <div class="hb-slideshow-dot" data-slide="4"></div>
                </div>
            </div>
        </div>

        <!-- Quick stats row -->
        <div class="hb-stats-row">
            <div class="hb-stat">
                <div class="hb-icon events" style="width:44px;height:44px;">
                    <svg viewBox="0 0 24 24" fill="none" stroke="#2ea44f" stroke-width="2"><path d="M8 2v4M16 2v4M3 10h18M5 4h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2z"/></svg>
                </div>
                <div>
                    <div class="hb-stat-num"><asp:Literal ID="litUpcomingEvents" runat="server" Text="3" /></div>
                    <div class="hb-stat-label">Upcoming Events</div>
                </div>
            </div>
            <div class="hb-stat">
                <div class="hb-icon birthday" style="width:44px;height:44px;">
                    <svg viewBox="0 0 24 24" fill="none" stroke="#e0455a" stroke-width="2"><path d="M12 6c1 0 1.5-1 1-2s-1-1-1-2c-.5 1-1 1-1 2s0 2 1 2zM4 22v-8a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8M2 22h20M4 14c1-1 2 1 3 0s2-1 3 0 2 1 3 0 2-1 3 0 2 1 3 0"/></svg>
                </div>
                <div>
                    <div class="hb-stat-num"><asp:Literal ID="litUpcomingBirthdays" runat="server" Text="2" /></div>
                    <div class="hb-stat-label">Birthdays This Week</div>
                </div>
            </div>
            <div class="hb-stat">
                <div class="hb-icon holiday" style="width:44px;height:44px;">
                    <svg viewBox="0 0 24 24" fill="none" stroke="#e8a33d" stroke-width="2"><circle cx="12" cy="12" r="10"/><path d="M12 6v6l4 2"/></svg>
                </div>
                <div>
                    <div class="hb-stat-num"><asp:Literal ID="litNextHoliday" runat="server" Text="15 Aug" /></div>
                    <div class="hb-stat-label">Next Holiday</div>
                </div>
            </div>
            <div class="hb-stat">
                <div class="hb-icon news" style="width:44px;height:44px;">
                    <svg viewBox="0 0 24 24" fill="none" stroke="#2f6fed" stroke-width="2"><path d="M4 4h16v14a2 2 0 0 1-2 2H4z"/><path d="M4 4v16M8 8h8M8 12h8M8 16h5"/></svg>
                </div>
                <div>
                    <div class="hb-stat-num"><asp:Literal ID="litNewsCount" runat="server" Text="4" /></div>
                    <div class="hb-stat-label">New Announcements</div>
                </div>
            </div>
        </div>

        <!-- Main banner cards: News, Birthdays, Events -->
        <div class="hb-grid">

            <!-- News / Announcements -->
            <div class="hb-card">
                <div class="hb-card-head">
                    <div class="hb-icon news">
                        <svg viewBox="0 0 24 24" fill="none" stroke="#2f6fed" stroke-width="2"><path d="M4 4h16v14a2 2 0 0 1-2 2H4z"/><path d="M4 4v16M8 8h8M8 12h8M8 16h5"/></svg>
                    </div>
                    <div style="flex:1;">
                        <div class="hb-card-title">Company News</div>
                        <div class="hb-card-sub">Latest updates & announcements</div>
                    </div>
                    <a class="hb-add-btn" href="#" onclick="openModal('modalNews'); return false;" title="Add News">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
                        Add News
                    </a>
                </div>
                <div class="hb-card-body">
                    <asp:Repeater ID="rptNews" runat="server">
                        <ItemTemplate>
                            <div class="hb-item">
                                <div class="hb-avatar" style="background:#2f6fed;">
                                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#fff" stroke-width="2"><path d="M4 4h16v14a2 2 0 0 1-2 2H4z"/></svg>
                                </div>
                                <div style="flex:1;">
                                    <div class="hb-item-title"><%# Eval("Title") %></div>
                                    <div class="hb-item-meta"><%# Eval("PostedOn") %></div>
                                </div>
                                <span class="hb-badge upcoming"><%# Eval("Tag") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <%-- Visible items (always shown) --%>
                    <div class="hb-item">
                        <div class="hb-avatar" style="background:#2f6fed;">HR</div>
                        <div style="flex:1;">
                            <div class="hb-item-title">New leave policy effective August 2026</div>
                            <div class="hb-item-meta">Posted 2 days ago</div>
                        </div>
                        <span class="hb-badge upcoming">Policy</span>
                    </div>
                    <div class="hb-item">
                        <div class="hb-avatar" style="background:#2ea44f;">IT</div>
                        <div style="flex:1;">
                            <div class="hb-item-title">Scheduled server maintenance this weekend</div>
                            <div class="hb-item-meta">Posted 4 days ago</div>
                        </div>
                        <span class="hb-badge date">Notice</span>
                    </div>
                    <div class="hb-item">
                        <div class="hb-avatar" style="background:#8a5fe0;">MG</div>
                        <div style="flex:1;">
                            <div class="hb-item-title">Q3 performance review cycle begins</div>
                            <div class="hb-item-meta">Posted 6 days ago</div>
                        </div>
                        <span class="hb-badge upcoming">Reminder</span>
                    </div>
                    <%-- Hidden extras --%>
                    <div class="hb-extra" id="newsExtra">
                        <div class="hb-item">
                            <div class="hb-avatar" style="background:#e8a33d;">FN</div>
                            <div style="flex:1;">
                                <div class="hb-item-title">Salary disbursement on 31st July 2026</div>
                                <div class="hb-item-meta">Posted 1 week ago</div>
                            </div>
                            <span class="hb-badge date">Finance</span>
                        </div>
                        <div class="hb-item">
                            <div class="hb-avatar" style="background:#e0455a;">AD</div>
                            <div style="flex:1;">
                                <div class="hb-item-title">Office relocation update — Block B, 3rd floor</div>
                                <div class="hb-item-meta">Posted 1 week ago</div>
                            </div>
                            <span class="hb-badge date">General</span>
                        </div>
                    </div>
                </div>
                <a class="hb-viewall" onclick="toggleExtra('newsExtra', this, 'View All News', 'Show Less')" href="#">View All News <span class="hb-extra-count">(+2)</span></a>
            </div>

            <!-- Birthdays -->
            <div class="hb-card">
                <div class="hb-card-head">
                    <div class="hb-icon birthday">
                        <svg viewBox="0 0 24 24" fill="none" stroke="#e0455a" stroke-width="2"><path d="M12 6c1 0 1.5-1 1-2s-1-1-1-2c-.5 1-1 1-1 2s0 2 1 2zM4 22v-8a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8M2 22h20M4 14c1-1 2 1 3 0s2-1 3 0 2 1 3 0 2-1 3 0 2 1 3 0"/></svg>
                    </div>
                    <div style="flex:1;">
                        <div class="hb-card-title">Upcoming Birthdays</div>
                        <div class="hb-card-sub">Celebrate with your teammates</div>
                    </div>
                    <a class="hb-add-btn" href="#" onclick="openModal('modalBirthday'); return false;" title="Add Birthday">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
                        Add Birthday
                    </a>
                </div>
                <div class="hb-card-body">
                    <asp:Repeater ID="rptBirthdays" runat="server">
                        <ItemTemplate>
                            <div class="hb-item">
                                <div class="hb-avatar"><%# Eval("Initials") %></div>
                                <div style="flex:1;">
                                    <div class="hb-item-title"><%# Eval("EmployeeName") %></div>
                                    <div class="hb-item-meta"><%# Eval("Department") %></div>
                                </div>
                                <span class="hb-badge today"><%# Eval("DateLabel") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <%-- Visible items --%>
                    <div class="hb-item">
                        <div class="hb-avatar" style="background:#e0455a;">RS</div>
                        <div style="flex:1;">
                            <div class="hb-item-title">Riya Sharma</div>
                            <div class="hb-item-meta">Human Resources</div>
                        </div>
                        <span class="hb-badge today">Today</span>
                    </div>
                    <div class="hb-item">
                        <div class="hb-avatar" style="background:#8a5fe0;">AK</div>
                        <div style="flex:1;">
                            <div class="hb-item-title">Aman Kothari</div>
                            <div class="hb-item-meta">Development</div>
                        </div>
                        <span class="hb-badge date">17 Jul</span>
                    </div>
                    <div class="hb-item">
                        <div class="hb-avatar" style="background:#2f6fed;">SP</div>
                        <div style="flex:1;">
                            <div class="hb-item-title">Sneha Patil</div>
                            <div class="hb-item-meta">Finance</div>
                        </div>
                        <span class="hb-badge date">19 Jul</span>
                    </div>
                    <%-- Hidden extras --%>
                    <div class="hb-extra" id="bdayExtra">
                        <div class="hb-item">
                            <div class="hb-avatar" style="background:#2ea44f;">VN</div>
                            <div style="flex:1;">
                                <div class="hb-item-title">Vikram Nair</div>
                                <div class="hb-item-meta">Operations</div>
                            </div>
                            <span class="hb-badge upcoming">23 Jul</span>
                        </div>
                        <div class="hb-item">
                            <div class="hb-avatar" style="background:#e8a33d;">PD</div>
                            <div style="flex:1;">
                                <div class="hb-item-title">Priya Desai</div>
                                <div class="hb-item-meta">Marketing</div>
                            </div>
                            <span class="hb-badge upcoming">28 Jul</span>
                        </div>
                    </div>
                </div>
                <a class="hb-viewall" onclick="toggleExtra('bdayExtra', this, 'View All Birthdays', 'Show Less')" href="#">View All Birthdays <span class="hb-extra-count">(+2)</span></a>
            </div>

            <!-- Events / Holidays -->
            <div class="hb-card">
                <div class="hb-card-head">
                    <div class="hb-icon events">
                        <svg viewBox="0 0 24 24" fill="none" stroke="#2ea44f" stroke-width="2"><path d="M8 2v4M16 2v4M3 10h18M5 4h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2z"/></svg>
                    </div>
                    <div style="flex:1;">
                        <div class="hb-card-title">Events &amp; Holidays</div>
                        <div class="hb-card-sub">What's coming up next</div>
                    </div>
                    <a class="hb-add-btn" href="#" onclick="openModal('modalEvent'); return false;" title="Add Event">
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/></svg>
                        Add Event
                    </a>
                </div>
                <div class="hb-card-body">
                    <asp:Repeater ID="rptEvents" runat="server">
                        <ItemTemplate>
                            <div class="hb-item">
                                <div class="hb-avatar" style="background:#2ea44f;">
                                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#fff" stroke-width="2"><path d="M8 2v4M16 2v4M3 10h18M5 4h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2z"/></svg>
                                </div>
                                <div style="flex:1;">
                                    <div class="hb-item-title"><%# Eval("EventName") %></div>
                                    <div class="hb-item-meta"><%# Eval("EventDate") %></div>
                                </div>
                                <span class="hb-badge upcoming"><%# Eval("EventType") %></span>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <%-- Visible items --%>
                    <div class="hb-item">
                        <div class="hb-avatar" style="background:#2f6fed;">18</div>
                        <div style="flex:1;">
                            <div class="hb-item-title">Team Building Activity</div>
                            <div class="hb-item-meta">18 July 2026 &middot; 2:00 PM</div>
                        </div>
                        <span class="hb-badge upcoming">Event</span>
                    </div>
                    <div class="hb-item">
                        <div class="hb-avatar" style="background:#2f6fed;">22</div>
                        <div style="flex:1;">
                            <div class="hb-item-title">Quarterly Town Hall</div>
                            <div class="hb-item-meta">22 July 2026 &middot; 4:00 PM</div>
                        </div>
                        <span class="hb-badge upcoming">Event</span>
                    </div>
                    <div class="hb-item">
                        <div class="hb-avatar" style="background:#8a5fe0;">31</div>
                        <div style="flex:1;">
                            <div class="hb-item-title">HR Policy Workshop</div>
                            <div class="hb-item-meta">31 July 2026 &middot; 10:00 AM</div>
                        </div>
                        <span class="hb-badge upcoming">Event</span>
                    </div>
                    <%-- Hidden extras --%>
                    <div class="hb-extra" id="eventsExtra">
                        <div class="hb-item">
                            <div class="hb-avatar" style="background:#e8a33d;">15</div>
                            <div style="flex:1;">
                                <div class="hb-item-title">Independence Day</div>
                                <div class="hb-item-meta">15 August 2026 &middot; Friday</div>
                            </div>
                            <span class="hb-badge date">Holiday</span>
                        </div>
                        <div class="hb-item">
                            <div class="hb-avatar" style="background:#e8a33d;">27</div>
                            <div style="flex:1;">
                                <div class="hb-item-title">Ganesh Chaturthi</div>
                                <div class="hb-item-meta">27 August 2026 &middot; Wednesday</div>
                            </div>
                            <span class="hb-badge date">Holiday</span>
                        </div>
                    </div>
                </div>
                <a class="hb-viewall" onclick="toggleExtra('eventsExtra', this, 'View Full Calendar', 'Show Less')" href="#">View Full Calendar <span class="hb-extra-count">(+2)</span></a>
            </div>

        </div>
    </div>

    <!-- Add News Modal -->
    <div class="hb-modal-overlay" id="modalNews" onclick="overlayClose(event, 'modalNews')">
        <div class="hb-modal">
            <div class="hb-modal-header">
                <div class="hb-icon news" style="flex-shrink:0;">
                    <svg viewBox="0 0 24 24" fill="none" stroke="#2f6fed" stroke-width="2"><path d="M4 4h16v14a2 2 0 0 1-2 2H4z"/><path d="M4 4v16M8 8h8M8 12h8M8 16h5"/></svg>
                </div>
                <div class="hb-modal-title">Add News / Announcement</div>
                <button class="hb-modal-close" onclick="closeModal('modalNews')">&#x2715;</button>
            </div>
            <div class="hb-modal-body">
                <div class="hb-field">
                    <label>Title <span style="color:#e0455a;">*</span></label>
                    <input type="text" id="newsTitle" placeholder="e.g. New leave policy update" />
                </div>
                <div class="hb-field-row">
                    <div class="hb-field">
                        <label>Category</label>
                        <select id="newsTag">
                            <option value="Policy">Policy</option>
                            <option value="Notice">Notice</option>
                            <option value="Reminder">Reminder</option>
                            <option value="Finance">Finance</option>
                            <option value="General">General</option>
                            <option value="IT">IT</option>
                        </select>
                    </div>
                    <div class="hb-field">
                        <label>Posted By</label>
                        <input type="text" id="newsPostedBy" placeholder="e.g. HR, Admin" />
                    </div>
                </div>
                <div class="hb-field">
                    <label>Description</label>
                    <textarea id="newsDesc" placeholder="Brief description (optional)"></textarea>
                </div>
            </div>
            <div class="hb-modal-footer">
                <button class="hb-btn hb-btn-ghost" onclick="closeModal('modalNews')">Cancel</button>
                <button class="hb-btn hb-btn-primary" onclick="saveNews()">Post Announcement</button>
            </div>
        </div>
    </div>

    <!-- Add Birthday Modal -->
    <div class="hb-modal-overlay" id="modalBirthday" onclick="overlayClose(event, 'modalBirthday')">
        <div class="hb-modal">
            <div class="hb-modal-header">
                <div class="hb-icon birthday" style="flex-shrink:0;">
                    <svg viewBox="0 0 24 24" fill="none" stroke="#e0455a" stroke-width="2"><path d="M12 6c1 0 1.5-1 1-2s-1-1-1-2c-.5 1-1 1-1 2s0 2 1 2zM4 22v-8a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8M2 22h20M4 14c1-1 2 1 3 0s2-1 3 0 2 1 3 0 2-1 3 0 2 1 3 0"/></svg>
                </div>
                <div class="hb-modal-title">Add Birthday</div>
                <button class="hb-modal-close" onclick="closeModal('modalBirthday')">&#x2715;</button>
            </div>
            <div class="hb-modal-body">
                <div class="hb-field">
                    <label>Employee Name <span style="color:#e0455a;">*</span></label>
                    <input type="text" id="bdayName" placeholder="Full name" />
                </div>
                <div class="hb-field-row">
                    <div class="hb-field">
                        <label>Department <span style="color:#e0455a;">*</span></label>
                        <select id="bdayDept">
                            <option value="">Select department</option>
                            <option>Human Resources</option>
                            <option>Development</option>
                            <option>Finance</option>
                            <option>Operations</option>
                            <option>Marketing</option>
                            <option>Sales</option>
                            <option>Administration</option>
                        </select>
                    </div>
                    <div class="hb-field">
                        <label>Date of Birth <span style="color:#e0455a;">*</span></label>
                        <input type="date" id="bdayDate" />
                    </div>
                </div>
            </div>
            <div class="hb-modal-footer">
                <button class="hb-btn hb-btn-ghost" onclick="closeModal('modalBirthday')">Cancel</button>
                <button class="hb-btn hb-btn-primary" onclick="saveBirthday()">Save Birthday</button>
            </div>
        </div>
    </div>

    <!-- Add Event / Holiday Modal -->
    <div class="hb-modal-overlay" id="modalEvent" onclick="overlayClose(event, 'modalEvent')">
        <div class="hb-modal">
            <div class="hb-modal-header">
                <div class="hb-icon events" style="flex-shrink:0;">
                    <svg viewBox="0 0 24 24" fill="none" stroke="#2ea44f" stroke-width="2"><path d="M8 2v4M16 2v4M3 10h18M5 4h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2z"/></svg>
                </div>
                <div class="hb-modal-title">Add Event / Holiday</div>
                <button class="hb-modal-close" onclick="closeModal('modalEvent')">&#x2715;</button>
            </div>
            <div class="hb-modal-body">
                <div class="hb-field-row">
                    <div class="hb-field">
                        <label>Type <span style="color:#e0455a;">*</span></label>
                        <select id="eventType" onchange="toggleEventTime()">
                            <option value="Event">Event</option>
                            <option value="Holiday">Holiday</option>
                        </select>
                    </div>
                    <div class="hb-field">
                        <label>Date <span style="color:#e0455a;">*</span></label>
                        <input type="date" id="eventDate" />
                    </div>
                </div>
                <div class="hb-field">
                    <label>Title <span style="color:#e0455a;">*</span></label>
                    <input type="text" id="eventTitle" placeholder="e.g. Quarterly Town Hall" />
                </div>
                <div class="hb-field" id="eventTimeField">
                    <label>Time</label>
                    <input type="time" id="eventTime" />
                </div>
                <div class="hb-field">
                    <label>Description</label>
                    <textarea id="eventDesc" placeholder="Additional details (optional)"></textarea>
                </div>
            </div>
            <div class="hb-modal-footer">
                <button class="hb-btn hb-btn-ghost" onclick="closeModal('modalEvent')">Cancel</button>
                <button class="hb-btn hb-btn-primary" onclick="saveEvent()">Save</button>
            </div>
        </div>
    </div>

    <script>
        function openModal(id) {
            document.getElementById(id).classList.add('active');
            document.body.style.overflow = 'hidden';
        }

        function closeModal(id) {
            document.getElementById(id).classList.remove('active');
            document.body.style.overflow = '';
        }

        function overlayClose(e, id) {
            if (e.target === document.getElementById(id)) closeModal(id);
        }

        function toggleExtra(id, link, labelCollapsed, labelExpanded) {
            var el = document.getElementById(id);
            var countEl = link.querySelector('.hb-extra-count');
            var expanded = el.style.display === 'block';
            el.style.display = expanded ? 'none' : 'block';
            var baseText = expanded ? labelCollapsed : labelExpanded;
            link.childNodes[0].textContent = baseText + ' ';
            if (countEl) countEl.style.display = expanded ? '' : 'none';
        }

        function toggleEventTime() {
            var type = document.getElementById('eventType').value;
            document.getElementById('eventTimeField').style.display = type === 'Holiday' ? 'none' : '';
        }

        function saveNews() {
            var title = document.getElementById('newsTitle').value.trim();
            if (!title) { alert('Please enter a title.'); return; }
            closeModal('modalNews');
            document.getElementById('newsTitle').value = '';
            document.getElementById('newsDesc').value = '';
        }

        function saveBirthday() {
            var name = document.getElementById('bdayName').value.trim();
            var dept = document.getElementById('bdayDept').value;
            var date = document.getElementById('bdayDate').value;
            if (!name || !dept || !date) { alert('Please fill in all required fields.'); return; }
            closeModal('modalBirthday');
            document.getElementById('bdayName').value = '';
            document.getElementById('bdayDept').value = '';
            document.getElementById('bdayDate').value = '';
        }

        function saveEvent() {
            var title = document.getElementById('eventTitle').value.trim();
            var date = document.getElementById('eventDate').value;
            if (!title || !date) { alert('Please fill in all required fields.'); return; }
            closeModal('modalEvent');
            document.getElementById('eventTitle').value = '';
            document.getElementById('eventDate').value = '';
            document.getElementById('eventTime').value = '';
            document.getElementById('eventDesc').value = '';
        }

        document.addEventListener('keydown', function (e) {
            if (e.key === 'Escape') {
                ['modalNews', 'modalBirthday', 'modalEvent'].forEach(closeModal);
            }
        });

        // Slideshow functionality
        let currentSlide = 0;
        const slides = document.querySelectorAll('.hb-slide');
        const dots = document.querySelectorAll('.hb-slideshow-dot');
        const totalSlides = slides.length;
        let slideshowInterval;

        function showSlide(index) {
            slides.forEach((slide, i) => {
                slide.classList.remove('active');
                dots[i].classList.remove('active');
            });
            
            if (index >= totalSlides) {
                currentSlide = 0;
            } else if (index < 0) {
                currentSlide = totalSlides - 1;
            } else {
                currentSlide = index;
            }
            
            slides[currentSlide].classList.add('active');
            dots[currentSlide].classList.add('active');
        }

        function nextSlide() {
            showSlide(currentSlide + 1);
        }

        function startSlideshow() {
            slideshowInterval = setInterval(nextSlide, 4000);
        }

        function stopSlideshow() {
            clearInterval(slideshowInterval);
        }

        // Dot click handlers
        dots.forEach((dot, index) => {
            dot.addEventListener('click', () => {
                stopSlideshow();
                showSlide(index);
                startSlideshow();
            });
        });

        // Start slideshow when page loads
        document.addEventListener('DOMContentLoaded', startSlideshow);
    </script>

</asp:Content>
