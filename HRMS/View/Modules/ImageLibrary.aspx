<%@ Page Title="" Language="C#" MasterPageFile="~/View/Layout/Site1.Master" AutoEventWireup="true" CodeBehind="ImageLibrary.aspx.cs" Inherits="HRMS.View.Modules.ImageLibrary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.all.min.js"></script>
    <style>
        .img-lib {
            --il-primary: #2563EB;
            --il-primary-dark: #1D4ED8;
            --il-heading: #1E293B;
            --il-muted: #64748B;
            --il-line: #E8EEF7;
            --il-panel: #F8FAFC;
        }

        .img-lib .il-card {
            background: #FFFFFF;
            border: 1px solid var(--il-line);
            border-radius: 12px;
            box-shadow: 0 10px 30px rgba(15, 23, 42, 0.05);
            padding: 28px 30px;
        }

        .img-lib .il-title {
            color: var(--il-heading);
            font-size: 22px;
            font-weight: 800;
            margin-bottom: 22px;
        }

        .img-lib .il-form {
            background: var(--il-panel);
            border: 1px solid var(--il-line);
            border-radius: 10px;
            padding: 20px;
        }

        .img-lib .il-label {
            color: var(--il-heading);
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 8px;
        }

        .img-lib .il-form .form-control,
        .img-lib .il-form .form-select {
            border: 1px solid #D9E2EF;
            border-radius: 8px;
            height: 42px;
        }

        .img-lib .il-hint {
            color: var(--il-muted);
            font-size: 12.5px;
            margin-top: 12px;
        }

        .img-lib .il-btn {
            background: var(--il-primary);
            border: none;
            border-radius: 8px;
            color: #FFFFFF;
            font-weight: 600;
            height: 42px;
            transition: background-color .18s ease, box-shadow .18s ease, transform .18s ease;
        }

        .img-lib .il-btn:hover {
            background: var(--il-primary-dark);
            box-shadow: 0 10px 20px rgba(37, 99, 235, 0.22);
            transform: translateY(-1px);
        }

        .img-lib .il-grid-wrap { margin-top: 24px; overflow-x: auto; }

        .img-lib .il-grid { border-collapse: collapse; width: 100%; }

        .img-lib .il-grid th {
            background: #F1F5FB;
            border-bottom: 1px solid var(--il-line);
            color: #334155;
            font-size: 13px;
            font-weight: 700;
            padding: 12px 14px;
            text-align: left;
        }

        .img-lib .il-grid td {
            border-bottom: 1px solid #EEF2F7;
            color: #334155;
            font-size: 14px;
            padding: 11px 14px;
            vertical-align: middle;
        }

        .img-lib .il-thumb {
            background: #FFFFFF;
            border: 1px solid var(--il-line);
            border-radius: 6px;
            max-height: 54px;
            max-width: 120px;
            object-fit: contain;
            padding: 4px;
        }

        .img-lib .il-del {
            background: #FEF2F2;
            border: 1px solid #FECACA;
            border-radius: 6px;
            color: #DC2626 !important;
            font-size: 13px;
            font-weight: 600;
            padding: 6px 12px;
            text-decoration: none;
        }

        .img-lib .il-del:hover { background: #FEE2E2; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="img-lib">
        <div class="row">
            <div class="col-12">
                <div class="il-card">
                    <div class="il-title">Image Library</div>

                    <div class="il-form">
                        <div class="row g-3 align-items-end">
                            <div class="col-md-3">
                                <div class="il-label">Image Type</div>
                                <asp:DropDownList ID="ddlImageType" runat="server" CssClass="form-select">
                                    <asp:ListItem Value="Logo">Logo</asp:ListItem>
                                    <asp:ListItem Value="Signature">Signature</asp:ListItem>
                                    <asp:ListItem Value="Other">Other</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-4">
                                <div class="il-label">Image Name</div>
                                <asp:TextBox ID="txtImageName" runat="server" CssClass="form-control" placeholder="e.g. Company Logo" />
                            </div>
                            <div class="col-md-3">
                                <div class="il-label">Image File</div>
                                <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" accept="image/*" />
                            </div>
                            <div class="col-md-2">
                                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="il-btn w-100" OnClick="btnSave_Click" />
                            </div>
                        </div>
                        <div class="il-hint">Accepted: PNG, JPG, GIF, SVG, WEBP, BMP. Max ~300 KB.</div>
                    </div>

                    <div class="il-grid-wrap">
                        <asp:GridView ID="gvImages" runat="server" AutoGenerateColumns="false" CssClass="il-grid"
                            GridLines="None" Width="100%" DataKeyNames="ImageId"
                            OnRowCommand="gvImages_RowCommand" EmptyDataText="No images added yet.">
                            <Columns>
                                <asp:TemplateField HeaderText="Preview">
                                    <ItemTemplate>
                                        <img class="il-thumb" src='<%# GetImageSrc(Eval("ContentType"), Eval("ImageBase64")) %>' alt="" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ImageType" HeaderText="Type" />
                                <asp:BoundField DataField="ImageName" HeaderText="Name" />
                                <asp:BoundField DataField="InsertedDate" HeaderText="Added On" DataFormatString="{0:dd-MM-yyyy HH:mm}" />
                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkDelete" runat="server" CssClass="il-del"
                                            CommandName="DeleteImage" CommandArgument='<%# Eval("ImageId") %>'
                                            OnClientClick="return confirm('Delete this image?');">
                                            <i class="fa fa-trash"></i>&nbsp;Delete
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
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
    </script>
</asp:Content>
