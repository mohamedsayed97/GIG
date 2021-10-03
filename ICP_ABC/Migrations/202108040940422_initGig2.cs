namespace ICP_ABC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initGig2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Banks",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Block",
                c => new
                    {
                        code = c.Int(nullable: false, identity: true),
                        cust_id = c.String(nullable: false, maxLength: 128),
                        fund_id = c.Int(nullable: false),
                        branch_id = c.Int(nullable: false),
                        block_date = c.DateTime(nullable: false),
                        qty_block = c.Decimal(nullable: false, precision: 18, scale: 2),
                        block_reson = c.String(maxLength: 50),
                        flag_tr = c.Short(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        BlockCmb = c.Int(nullable: false),
                        blockauth = c.Boolean(nullable: false),
                        unit_price = c.Int(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auther = c.String(),
                        Maker = c.String(),
                    })
                .PrimaryKey(t => t.code)
                .ForeignKey("dbo.Branch", t => t.branch_id, cascadeDelete: true)
                .ForeignKey("dbo.Customer", t => t.cust_id, cascadeDelete: true)
                .ForeignKey("dbo.fund", t => t.fund_id, cascadeDelete: true)
                .Index(t => t.cust_id)
                .Index(t => t.fund_id)
                .Index(t => t.branch_id);
            
            CreateTable(
                "dbo.Branch",
                c => new
                    {
                        BranchID = c.Int(nullable: false, identity: true),
                        branchcode = c.String(nullable: false, maxLength: 10),
                        BName = c.String(nullable: false, maxLength: 50),
                        UserID = c.String(nullable: false),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.BranchID)
                .Index(t => t.branchcode, unique: true);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Code = c.String(nullable: false, maxLength: 4),
                        UserName = c.String(nullable: false, maxLength: 256),
                        FullName = c.String(nullable: false, maxLength: 100),
                        ExpireDate = c.DateTime(nullable: false),
                        BranchId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                        TitleId = c.Int(nullable: false),
                        BranchRight = c.Boolean(nullable: false),
                        UnBlockRight = c.Boolean(nullable: false),
                        IsLocked = c.Boolean(nullable: false),
                        EditFlag = c.Boolean(nullable: false),
                        DeleteFlag = c.Int(nullable: false),
                        Maker = c.String(),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        IsLogged = c.Boolean(nullable: false),
                        Auther = c.String(),
                        UserId = c.String(maxLength: 128),
                        CreationDate = c.DateTime(nullable: false),
                        SysDate = c.DateTime(nullable: false),
                        CloseDueDate = c.DateTime(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        Calendar_CalendarID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.Titles", t => t.TitleId, cascadeDelete: true)
                .ForeignKey("dbo.UserGroup", t => t.GroupId, cascadeDelete: true)
                .ForeignKey("dbo.Branch", t => t.BranchId, cascadeDelete: true)
                .ForeignKey("dbo.Calendar", t => t.Calendar_CalendarID)
                .Index(t => t.Code, unique: true)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.BranchId)
                .Index(t => t.GroupId)
                .Index(t => t.TitleId)
                .Index(t => t.UserId)
                .Index(t => t.Calendar_CalendarID);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.GroupRights",
                c => new
                    {
                        GroupRightID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        Create = c.Boolean(nullable: false),
                        Update = c.Boolean(nullable: false),
                        Delete = c.Boolean(nullable: false),
                        Read = c.Boolean(nullable: false),
                        FormID = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        GroupId = c.Int(nullable: false),
                        Authorized = c.Boolean(nullable: false),
                        None = c.Boolean(nullable: false),
                        Check = c.Boolean(nullable: false),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                    })
                .PrimaryKey(t => t.GroupRightID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.Screens", t => t.FormID, cascadeDelete: true)
                .ForeignKey("dbo.UserGroup", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.FormID)
                .Index(t => t.UserId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.Screens",
                c => new
                    {
                        ScreenID = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 10),
                        Name = c.String(maxLength: 15),
                    })
                .PrimaryKey(t => t.ScreenID)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "dbo.UserGroup",
                c => new
                    {
                        GroupID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        Name = c.String(nullable: false, maxLength: 50),
                        UserID = c.String(nullable: false),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        LockGroup = c.Boolean(nullable: false),
                        SysDate = c.DateTime(nullable: false),
                        HasGroupRight = c.Boolean(nullable: false),
                        HasFundRight = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.GroupID)
                .Index(t => t.Code, unique: true)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.FundRight",
                c => new
                    {
                        FundRightID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        FundRightAuth = c.Boolean(nullable: false),
                        FundID = c.Int(nullable: false),
                        GroupID = c.Int(nullable: false),
                        UserID = c.String(maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                    })
                .PrimaryKey(t => t.FundRightID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.fund", t => t.FundID, cascadeDelete: true)
                .ForeignKey("dbo.UserGroup", t => t.GroupID, cascadeDelete: true)
                .Index(t => t.FundID)
                .Index(t => t.GroupID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.fund",
                c => new
                    {
                        FundID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        fname = c.String(maxLength: 100),
                        CurrencyID = c.Int(nullable: false),
                        SponsorID = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        ParView = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Units = c.Int(nullable: false),
                        SubFeesBar = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RedFeesBar = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinInd = c.Int(nullable: false),
                        MaxInd = c.Int(nullable: false),
                        MinCor = c.Int(nullable: false),
                        MaxCor = c.Int(nullable: false),
                        OtherSubFees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OtherRedFees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        AdminFees = c.Int(nullable: false),
                        EarlyFees = c.Int(nullable: false),
                        SysDate = c.DateTime(nullable: false),
                        EntryDate = c.DateTime(nullable: false),
                        UserID = c.String(maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        inputer = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        Timestamp = c.DateTime(nullable: false),
                        Nav = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Remark = c.String(),
                        ISIN = c.String(maxLength: 10),
                        InvDate = c.DateTime(nullable: false),
                        FundAcc = c.String(),
                        MarkFees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SubFeesAcc = c.String(),
                        RedemFeesAcc = c.String(),
                        ManageFeesAcc = c.String(),
                        AdminFeesAcc = c.String(),
                        OtherSubAcc = c.String(),
                        OtherRedAcc = c.String(),
                        EarlyFeesAcc = c.String(),
                        ProductEligable = c.Int(nullable: false),
                        GuaranteeNotePer = c.String(),
                        GuranteeNote = c.String(),
                        CboType = c.Int(nullable: false),
                        UpFrontFees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UpFrontAcc = c.String(),
                        FreeText = c.String(),
                        CouponBox = c.Int(nullable: false),
                        UserLog = c.String(),
                        HasFundTime = c.Boolean(nullable: false),
                        HasICPrice = c.Boolean(nullable: false),
                        CustTypeID = c.Int(nullable: false),
                        sub_fees_type = c.Boolean(nullable: false),
                        Min_Sub_Fees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        acc_Sub_Fees_Type = c.Short(nullable: false),
                        Max_Sub_Fees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        max_sub_fees_type = c.Boolean(nullable: false),
                        Red_Fees_Type = c.Boolean(nullable: false),
                        Min_Red_Fees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        acc_red_fees_type = c.Int(nullable: false),
                        Max_Red_Fees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Max_Red_Fees_type = c.Decimal(nullable: false, precision: 18, scale: 2),
                        fmatlead = c.DateTime(nullable: false),
                        ceiling = c.Decimal(nullable: false, precision: 18, scale: 2),
                        cper_flag = c.Int(nullable: false),
                        no_ics = c.Int(nullable: false),
                        nomval = c.Decimal(nullable: false, precision: 18, scale: 2),
                        subper = c.Decimal(nullable: false, precision: 18, scale: 2),
                        redper = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FUNSDND = c.Int(nullable: false),
                        min_pos = c.Int(nullable: false),
                        Min_hold_units = c.Int(nullable: false),
                        FAccType = c.String(),
                        Fund_UAccNo = c.String(),
                        susp_acc_no = c.String(),
                        Inception_Date = c.DateTime(nullable: false),
                        TransOrder = c.Int(nullable: false),
                        PriceTol = c.Decimal(nullable: false, precision: 18, scale: 2),
                        min_Date = c.DateTime(nullable: false),
                        fNameAr = c.String(),
                        amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Monthly = c.Int(nullable: false),
                        Weekly = c.Int(nullable: false),
                        MonthlyOption = c.Int(nullable: false),
                        ICprice = c.Int(nullable: false),
                        SpecialCase = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.FundID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.Currency", t => t.CurrencyID, cascadeDelete: true)
                .ForeignKey("dbo.custtype", t => t.CustTypeID, cascadeDelete: true)
                .ForeignKey("dbo.Sponsor", t => t.SponsorID, cascadeDelete: true)
                .Index(t => t.Code, unique: true)
                .Index(t => t.fname, unique: true)
                .Index(t => t.CurrencyID)
                .Index(t => t.SponsorID)
                .Index(t => t.UserID)
                .Index(t => t.ISIN, unique: true)
                .Index(t => t.CustTypeID);
            
            CreateTable(
                "dbo.Currency",
                c => new
                    {
                        CurrencyID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        Name = c.String(nullable: false, maxLength: 50),
                        ShortName = c.String(nullable: false, maxLength: 50),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UserID = c.String(nullable: false, maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CurrencyID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID, cascadeDelete: false)
                .Index(t => t.Code, unique: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.custtype",
                c => new
                    {
                        CustTypeID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        Name = c.String(nullable: false, maxLength: 50),
                        LOP = c.Int(nullable: false),
                        UserID = c.String(maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Boolean(nullable: false),
                        Maker = c.String(nullable: false),
                        Check = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CustTypeID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.Code, unique: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Sponsor",
                c => new
                    {
                        SponsorID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        Name = c.String(nullable: false, maxLength: 50),
                        UserID = c.String(nullable: false, maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SponsorID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID, cascadeDelete: false)
                .Index(t => t.Code, unique: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: false)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Titles",
                c => new
                    {
                        TitleID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        UserID = c.String(nullable: false),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Boolean(nullable: false),
                        Maker = c.String(nullable: false),
                        Check = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TitleID);
            
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        CustomerID = c.String(nullable: false, maxLength: 128),
                        Id = c.Int(nullable: false, identity: true),
                        tel1 = c.String(),
                        EnName = c.String(),
                        ArName = c.String(),
                        EnAddress1 = c.String(),
                        ArAddress1 = c.String(),
                        Email1 = c.String(),
                        DateOfHiring = c.DateTime(nullable: false),
                        DateOfContribute = c.DateTime(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        Salary = c.Int(nullable: false),
                        NationalityId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        IdNumber = c.String(),
                        UserID = c.String(maxLength: 128),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        entry_date = c.DateTime(nullable: false),
                        EnAddress2 = c.String(),
                        ArAddress2 = c.String(),
                        Email2 = c.String(),
                        Email3 = c.String(),
                        Email4 = c.String(),
                        PostalCode = c.Int(),
                        CRNumber = c.String(),
                        Sector = c.String(),
                        IssuanceDate = c.DateTime(nullable: false),
                        tel2 = c.String(maxLength: 20, fixedLength: true, unicode: false),
                        tel3 = c.Int(),
                        Fax1 = c.Int(),
                        Fax2 = c.Int(),
                        Fax3 = c.Int(),
                        CustTypeId = c.Int(),
                        BranchId = c.Int(),
                        idType = c.Int(),
                        EditFlag = c.Boolean(),
                        DeletFlag = c.Int(),
                        AR_PrimaryAddress = c.String(),
                        EN_PrimaryAddress = c.String(),
                    })
                .PrimaryKey(t => t.CustomerID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.Branch", t => t.BranchId)
                .ForeignKey("dbo.City", t => t.CityId, cascadeDelete: true)
                .ForeignKey("dbo.custtype", t => t.CustTypeId)
                .ForeignKey("dbo.UserIdentityTypes", t => t.idType)
                .ForeignKey("dbo.Nationality", t => t.NationalityId, cascadeDelete: true)
                .Index(t => t.NationalityId)
                .Index(t => t.CityId)
                .Index(t => t.UserID)
                .Index(t => t.CustTypeId)
                .Index(t => t.BranchId)
                .Index(t => t.idType);
            
            CreateTable(
                "dbo.City",
                c => new
                    {
                        CityID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        Name = c.String(nullable: false, maxLength: 50),
                        UserID = c.String(nullable: false, maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CityID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID, cascadeDelete: false)
                .Index(t => t.Code, unique: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.UserIdentityTypes",
                c => new
                    {
                        UserIdentityTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                    })
                .PrimaryKey(t => t.UserIdentityTypeID);
            
            CreateTable(
                "dbo.Nationality",
                c => new
                    {
                        NationalityID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        Name = c.String(nullable: false, maxLength: 50),
                        Default = c.Boolean(nullable: false),
                        ForeignCurrency = c.Boolean(nullable: false),
                        UserID = c.String(nullable: false, maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.NationalityID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID, cascadeDelete: false)
                .Index(t => t.Code, unique: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.blocktable",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        code = c.Int(nullable: false),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Calendar",
                c => new
                    {
                        CalendarID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        Vacation_Name = c.String(nullable: false, maxLength: 50),
                        UserID = c.String(nullable: false),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        Vacation_date = c.DateTime(nullable: false),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CalendarID)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "dbo.Client_CodeMap",
                c => new
                    {
                        ClientID = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        ICproCID = c.String(maxLength: 20),
                        CoreCID = c.String(maxLength: 20),
                        auth = c.Int(nullable: false),
                        Auther = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        EditFlag = c.Boolean(nullable: false),
                        DeleteFlag = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClientID);
            
            CreateTable(
                "dbo.Client_CodeMap_LOG",
                c => new
                    {
                        ClientLogID = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        ICproCID = c.String(maxLength: 20),
                        CoreCID = c.String(maxLength: 20),
                        auth = c.Int(nullable: false),
                        Auther = c.String(maxLength: 50),
                        StartDate = c.DateTime(nullable: false),
                        V8ename = c.String(),
                        V8eaddress = c.String(),
                        V8emaddress = c.String(),
                        V8City = c.String(),
                        V8idnumber = c.String(),
                        V8idtype = c.String(),
                        V8nation = c.String(),
                        V8cboType = c.String(),
                        V8branch = c.String(),
                        V8tel = c.String(),
                        V8fax = c.String(),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        EditFlag = c.Boolean(nullable: false),
                        DeleteFlag = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClientLogID);
            
            CreateTable(
                "dbo.Days",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Day_Id = c.Int(nullable: false),
                        DayName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FundAuthDays",
                c => new
                    {
                        Code = c.Int(nullable: false, identity: true),
                        Day_Id = c.Int(nullable: false),
                        Sub_Red = c.Int(nullable: false),
                        FundId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Code)
                .ForeignKey("dbo.fund", t => t.FundId, cascadeDelete: true)
                .Index(t => t.FundId);
            
            CreateTable(
                "dbo.FundDays",
                c => new
                    {
                        Iden = c.Int(nullable: false, identity: true),
                        Day_Id = c.Int(nullable: false),
                        Sub_Red = c.Int(nullable: false),
                        FundId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Iden)
                .ForeignKey("dbo.fund", t => t.FundId, cascadeDelete: true)
                .Index(t => t.FundId);
            
            CreateTable(
                "dbo.FundTime",
                c => new
                    {
                        FundTimeID = c.Int(nullable: false, identity: true),
                        fund_time = c.Time(nullable: false, precision: 7),
                        FundId = c.Int(nullable: false),
                        UserID = c.String(maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                    })
                .PrimaryKey(t => t.FundTimeID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.fund", t => t.FundId, cascadeDelete: true)
                .Index(t => t.FundId)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.ICPrice",
                c => new
                    {
                        ICPriceID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 4),
                        FundId = c.Int(nullable: false),
                        ICDate = c.DateTime(nullable: false),
                        ProcessingDate = c.DateTime(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 5),
                        UserID = c.String(nullable: false, maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Navauth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ICPriceID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID, cascadeDelete: false)
                .ForeignKey("dbo.fund", t => t.FundId, cascadeDelete: true)
                .Index(t => t.FundId)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.LastCode",
                c => new
                    {
                        iden = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.iden);
            
            CreateTable(
                "dbo.Redemption",
                c => new
                    {
                        code = c.Int(nullable: false),
                        branch_id = c.Int(nullable: false),
                        cust_acc_no = c.String(maxLength: 30),
                        cust_id = c.String(maxLength: 128),
                        fund_id = c.Int(nullable: false),
                        appliction_no = c.Int(nullable: false),
                        pur_date = c.DateTime(),
                        units = c.Decimal(precision: 18, scale: 5),
                        amount_3 = c.Decimal(nullable: false, precision: 18, scale: 2),
                        sub_fees = c.Decimal(nullable: false, precision: 18, scale: 5),
                        system_date = c.DateTime(),
                        nav_date = c.DateTime(),
                        ProcessingDate = c.DateTime(),
                        Nav_Ddate = c.DateTime(),
                        inputer = c.String(),
                        auth = c.Short(nullable: false),
                        auther = c.String(),
                        flag_tr = c.Short(nullable: false),
                        pay_method = c.Short(nullable: false),
                        pay_no = c.String(maxLength: 50),
                        pay_bank = c.String(maxLength: 30),
                        total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NAV = c.Decimal(nullable: false, precision: 18, scale: 5),
                        other_fees = c.Decimal(nullable: false, precision: 18, scale: 5),
                        delreason = c.Int(nullable: false),
                        unauth = c.Int(nullable: false),
                        Flag = c.Int(nullable: false),
                        UserID = c.String(maxLength: 128),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        SysDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.code)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.Branch", t => t.branch_id, cascadeDelete: true)
                .ForeignKey("dbo.Customer", t => t.cust_id)
                .ForeignKey("dbo.fund", t => t.fund_id, cascadeDelete: true)
                .Index(t => t.branch_id)
                .Index(t => t.cust_id)
                .Index(t => t.fund_id)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Subscription",
                c => new
                    {
                        code = c.Int(nullable: false),
                        branch_id = c.Int(),
                        cust_acc_no = c.String(maxLength: 30),
                        cust_id = c.String(maxLength: 128),
                        fund_id = c.Int(),
                        appliction_no = c.Int(),
                        pur_date = c.DateTime(),
                        units = c.Decimal(precision: 18, scale: 2),
                        amount_3 = c.Decimal(precision: 18, scale: 2),
                        sub_fees = c.Decimal(precision: 18, scale: 5),
                        nav_date = c.DateTime(),
                        Nav_Ddate = c.DateTime(),
                        ProcessingDate = c.DateTime(),
                        system_date = c.DateTime(),
                        inputer = c.String(),
                        auth = c.Short(),
                        auther = c.String(),
                        flag_tr = c.Short(),
                        pay_method = c.Short(),
                        pay_no = c.String(maxLength: 50),
                        pay_bank = c.String(maxLength: 50),
                        total = c.Decimal(nullable: false, precision: 25, scale: 2),
                        NAV = c.Decimal(nullable: false, precision: 18, scale: 5),
                        other_fees = c.Decimal(precision: 18, scale: 5),
                        delreason = c.Int(nullable: false),
                        unauth = c.Int(),
                        Flag = c.Int(),
                        UserID = c.String(maxLength: 128),
                        Chk = c.Boolean(),
                        Checker = c.String(),
                        SysDate = c.DateTime(nullable: false),
                        GTF_Flag = c.Short(),
                    })
                .PrimaryKey(t => t.code)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.Branch", t => t.branch_id)
                .ForeignKey("dbo.Customer", t => t.cust_id)
                .ForeignKey("dbo.fund", t => t.fund_id)
                .Index(t => t.branch_id)
                .Index(t => t.cust_id)
                .Index(t => t.fund_id)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Trans",
                c => new
                    {
                        code = c.Int(nullable: false, identity: true),
                        cust_id = c.String(),
                        cust_acc_no = c.String(maxLength: 30),
                        fund_id = c.Int(nullable: false),
                        value_date = c.DateTime(),
                        entry_date = c.DateTime(),
                        quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        unit_price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        pur_sal = c.Short(nullable: false),
                        branch_id = c.Int(nullable: false),
                        payment_met = c.Short(nullable: false),
                        time_stamp = c.DateTime(),
                        user_id = c.Int(nullable: false),
                        auth = c.Short(nullable: false),
                        auther = c.Int(nullable: false),
                        fees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        other_fees = c.Decimal(precision: 18, scale: 2),
                        inputer = c.String(),
                        flag_tr = c.Short(nullable: false),
                        curr_id = c.Int(nullable: false),
                        transid = c.Int(nullable: false),
                        mark_fees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        admin_fees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        early_fees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        system_date = c.DateTime(),
                        upfront_fees = c.Decimal(nullable: false, precision: 18, scale: 2),
                        total_value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SysDate = c.DateTime(nullable: false),
                        Flag = c.Int(nullable: false),
                        UserID = c.String(),
                        Nav_Ddate = c.DateTime(),
                        ProcessingDate = c.DateTime(),
                        GTF_Flag = c.Short(),
                    })
                .PrimaryKey(t => t.code);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false),
                        name = c.String(maxLength: 50),
                        branch_id = c.Int(nullable: false),
                        group_id = c.Int(nullable: false),
                        branch_right = c.Boolean(nullable: false),
                        Position = c.Int(nullable: false),
                        pass = c.String(maxLength: 10),
                        exp_date = c.DateTime(nullable: false),
                        EditFlag = c.Boolean(nullable: false),
                        DeleteFlag = c.Int(nullable: false),
                        Admin = c.Boolean(nullable: false),
                        LockUser = c.Boolean(nullable: false),
                        Userid = c.String(),
                        sys_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserSecurity",
                c => new
                    {
                        code = c.Int(nullable: false, identity: true),
                        NumberOfTrials = c.Int(nullable: false),
                        ExpireInterval = c.Int(nullable: false),
                        SecLevel = c.Int(nullable: false),
                        User_log = c.String(nullable: false, maxLength: 128),
                        EditFlag = c.Boolean(nullable: false),
                        DeletFlag = c.Int(nullable: false),
                        Maker = c.String(nullable: false),
                        Chk = c.Boolean(nullable: false),
                        Checker = c.String(),
                        Auth = c.Boolean(nullable: false),
                        Auther = c.String(),
                        SysDate = c.DateTime(nullable: false),
                        ViewTransaction = c.Boolean(nullable: false),
                        CreateTransaction = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.code)
                .ForeignKey("dbo.AspNetUsers", t => t.User_log, cascadeDelete: true)
                .Index(t => t.User_log);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserSecurity", "User_log", "dbo.AspNetUsers");
            DropForeignKey("dbo.Subscription", "fund_id", "dbo.fund");
            DropForeignKey("dbo.Subscription", "cust_id", "dbo.Customer");
            DropForeignKey("dbo.Subscription", "branch_id", "dbo.Branch");
            DropForeignKey("dbo.Subscription", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Redemption", "fund_id", "dbo.fund");
            DropForeignKey("dbo.Redemption", "cust_id", "dbo.Customer");
            DropForeignKey("dbo.Redemption", "branch_id", "dbo.Branch");
            DropForeignKey("dbo.Redemption", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.ICPrice", "FundId", "dbo.fund");
            DropForeignKey("dbo.ICPrice", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.FundTime", "FundId", "dbo.fund");
            DropForeignKey("dbo.FundTime", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.FundDays", "FundId", "dbo.fund");
            DropForeignKey("dbo.FundAuthDays", "FundId", "dbo.fund");
            DropForeignKey("dbo.AspNetUsers", "Calendar_CalendarID", "dbo.Calendar");
            DropForeignKey("dbo.Block", "fund_id", "dbo.fund");
            DropForeignKey("dbo.Block", "cust_id", "dbo.Customer");
            DropForeignKey("dbo.Customer", "NationalityId", "dbo.Nationality");
            DropForeignKey("dbo.Nationality", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Customer", "idType", "dbo.UserIdentityTypes");
            DropForeignKey("dbo.Customer", "CustTypeId", "dbo.custtype");
            DropForeignKey("dbo.Customer", "CityId", "dbo.City");
            DropForeignKey("dbo.City", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Customer", "BranchId", "dbo.Branch");
            DropForeignKey("dbo.Customer", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Block", "branch_id", "dbo.Branch");
            DropForeignKey("dbo.AspNetUsers", "BranchId", "dbo.Branch");
            DropForeignKey("dbo.AspNetUsers", "GroupId", "dbo.UserGroup");
            DropForeignKey("dbo.AspNetUsers", "TitleId", "dbo.Titles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.GroupRights", "GroupId", "dbo.UserGroup");
            DropForeignKey("dbo.FundRight", "GroupID", "dbo.UserGroup");
            DropForeignKey("dbo.FundRight", "FundID", "dbo.fund");
            DropForeignKey("dbo.fund", "SponsorID", "dbo.Sponsor");
            DropForeignKey("dbo.Sponsor", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.fund", "CustTypeID", "dbo.custtype");
            DropForeignKey("dbo.custtype", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.fund", "CurrencyID", "dbo.Currency");
            DropForeignKey("dbo.Currency", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.fund", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.FundRight", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.GroupRights", "FormID", "dbo.Screens");
            DropForeignKey("dbo.GroupRights", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserSecurity", new[] { "User_log" });
            DropIndex("dbo.Subscription", new[] { "UserID" });
            DropIndex("dbo.Subscription", new[] { "fund_id" });
            DropIndex("dbo.Subscription", new[] { "cust_id" });
            DropIndex("dbo.Subscription", new[] { "branch_id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Redemption", new[] { "UserID" });
            DropIndex("dbo.Redemption", new[] { "fund_id" });
            DropIndex("dbo.Redemption", new[] { "cust_id" });
            DropIndex("dbo.Redemption", new[] { "branch_id" });
            DropIndex("dbo.ICPrice", new[] { "UserID" });
            DropIndex("dbo.ICPrice", new[] { "FundId" });
            DropIndex("dbo.FundTime", new[] { "UserID" });
            DropIndex("dbo.FundTime", new[] { "FundId" });
            DropIndex("dbo.FundDays", new[] { "FundId" });
            DropIndex("dbo.FundAuthDays", new[] { "FundId" });
            DropIndex("dbo.Calendar", new[] { "Code" });
            DropIndex("dbo.Nationality", new[] { "UserID" });
            DropIndex("dbo.Nationality", new[] { "Code" });
            DropIndex("dbo.City", new[] { "UserID" });
            DropIndex("dbo.City", new[] { "Code" });
            DropIndex("dbo.Customer", new[] { "idType" });
            DropIndex("dbo.Customer", new[] { "BranchId" });
            DropIndex("dbo.Customer", new[] { "CustTypeId" });
            DropIndex("dbo.Customer", new[] { "UserID" });
            DropIndex("dbo.Customer", new[] { "CityId" });
            DropIndex("dbo.Customer", new[] { "NationalityId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Sponsor", new[] { "UserID" });
            DropIndex("dbo.Sponsor", new[] { "Code" });
            DropIndex("dbo.custtype", new[] { "UserID" });
            DropIndex("dbo.custtype", new[] { "Code" });
            DropIndex("dbo.Currency", new[] { "UserID" });
            DropIndex("dbo.Currency", new[] { "Code" });
            DropIndex("dbo.fund", new[] { "CustTypeID" });
            DropIndex("dbo.fund", new[] { "ISIN" });
            DropIndex("dbo.fund", new[] { "UserID" });
            DropIndex("dbo.fund", new[] { "SponsorID" });
            DropIndex("dbo.fund", new[] { "CurrencyID" });
            DropIndex("dbo.fund", new[] { "fname" });
            DropIndex("dbo.fund", new[] { "Code" });
            DropIndex("dbo.FundRight", new[] { "UserID" });
            DropIndex("dbo.FundRight", new[] { "GroupID" });
            DropIndex("dbo.FundRight", new[] { "FundID" });
            DropIndex("dbo.UserGroup", new[] { "Name" });
            DropIndex("dbo.UserGroup", new[] { "Code" });
            DropIndex("dbo.Screens", new[] { "Code" });
            DropIndex("dbo.GroupRights", new[] { "GroupId" });
            DropIndex("dbo.GroupRights", new[] { "UserId" });
            DropIndex("dbo.GroupRights", new[] { "FormID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "Calendar_CalendarID" });
            DropIndex("dbo.AspNetUsers", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "TitleId" });
            DropIndex("dbo.AspNetUsers", new[] { "GroupId" });
            DropIndex("dbo.AspNetUsers", new[] { "BranchId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "Code" });
            DropIndex("dbo.Branch", new[] { "branchcode" });
            DropIndex("dbo.Block", new[] { "branch_id" });
            DropIndex("dbo.Block", new[] { "fund_id" });
            DropIndex("dbo.Block", new[] { "cust_id" });
            DropTable("dbo.UserSecurity");
            DropTable("dbo.Users");
            DropTable("dbo.Trans");
            DropTable("dbo.Subscription");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Redemption");
            DropTable("dbo.LastCode");
            DropTable("dbo.ICPrice");
            DropTable("dbo.FundTime");
            DropTable("dbo.FundDays");
            DropTable("dbo.FundAuthDays");
            DropTable("dbo.Days");
            DropTable("dbo.Client_CodeMap_LOG");
            DropTable("dbo.Client_CodeMap");
            DropTable("dbo.Calendar");
            DropTable("dbo.blocktable");
            DropTable("dbo.Nationality");
            DropTable("dbo.UserIdentityTypes");
            DropTable("dbo.City");
            DropTable("dbo.Customer");
            DropTable("dbo.Titles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Sponsor");
            DropTable("dbo.custtype");
            DropTable("dbo.Currency");
            DropTable("dbo.fund");
            DropTable("dbo.FundRight");
            DropTable("dbo.UserGroup");
            DropTable("dbo.Screens");
            DropTable("dbo.GroupRights");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Branch");
            DropTable("dbo.Block");
            DropTable("dbo.Banks");
        }
    }
}
