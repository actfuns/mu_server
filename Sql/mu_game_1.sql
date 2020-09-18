/*
 Navicat Premium Data Transfer

 Source Server         : test
 Source Server Type    : MySQL
 Source Server Version : 50547
 Source Host           : localhost:3306
 Source Schema         : mu_game_1

 Target Server Type    : MySQL
 Target Server Version : 50547
 File Encoding         : 65001

 Date: 18/09/2020 10:29:12
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for t_5v5_game_log
-- ----------------------------
DROP TABLE IF EXISTS `t_5v5_game_log`;
CREATE TABLE `t_5v5_game_log`  (
  `rid` int(11) NOT NULL,
  `zoneid1` int(11) NOT NULL,
  `rolename1` char(32) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `zoneid2` int(11) NOT NULL,
  `rolename2` char(32) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `success` tinyint(4) NOT NULL,
  `duanweijifenaward` int(11) NOT NULL,
  `rongyaoaward` int(11) NOT NULL,
  `endtime` datetime NOT NULL DEFAULT '2001-11-11 00:00:00',
  INDEX `idx_rid_endtime`(`rid`, `endtime`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_bin ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_activate
-- ----------------------------
DROP TABLE IF EXISTS `t_activate`;
CREATE TABLE `t_activate`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userID` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `zoneID` int(11) NULL DEFAULT NULL,
  `roleID` int(11) NULL DEFAULT NULL,
  `logTime` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `userID_dex`(`userID`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_adorationinfo
-- ----------------------------
DROP TABLE IF EXISTS `t_adorationinfo`;
CREATE TABLE `t_adorationinfo`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `adorationroleid` int(11) NOT NULL DEFAULT 0,
  `dayid` int(11) NOT NULL,
  PRIMARY KEY (`roleid`, `adorationroleid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_alchemy
-- ----------------------------
DROP TABLE IF EXISTS `t_alchemy`;
CREATE TABLE `t_alchemy`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `element` int(11) NOT NULL DEFAULT 0,
  `dayid` int(11) NOT NULL DEFAULT 0,
  `value` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `todaycost` char(155) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `histcost` char(155) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `rollback` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_ally_log
-- ----------------------------
DROP TABLE IF EXISTS `t_ally_log`;
CREATE TABLE `t_ally_log`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `myUnionID` int(11) NULL DEFAULT NULL,
  `unionID` int(11) NULL DEFAULT NULL,
  `unionZoneID` int(11) NULL DEFAULT NULL,
  `unionName` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `logTime` datetime NULL DEFAULT NULL,
  `logState` int(11) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_id_time`(`myUnionID`, `logTime`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_baitanbuy
-- ----------------------------
DROP TABLE IF EXISTS `t_baitanbuy`;
CREATE TABLE `t_baitanbuy`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `otherroleid` int(11) NOT NULL DEFAULT 0,
  `otherrname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `forgelevel` int(11) NOT NULL DEFAULT 0,
  `totalprice` int(11) NOT NULL DEFAULT 0,
  `leftyuanbao` int(11) NOT NULL DEFAULT 0,
  `buytime` datetime NULL,
  `yinliang` int(11) NOT NULL DEFAULT 0,
  `left_yinliang` int(11) NOT NULL DEFAULT 0,
  `tax` int(11) NOT NULL DEFAULT 0,
  `excellenceinfo` int(11) NOT NULL DEFAULT 0,
  `washprops` varchar(256) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `rid_idx`(`rid`) USING BTREE,
  INDEX `otherrid_idx`(`otherroleid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 707 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_ban_check
-- ----------------------------
DROP TABLE IF EXISTS `t_ban_check`;
CREATE TABLE `t_ban_check`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `roleID` int(11) NOT NULL DEFAULT 0,
  `banIDs` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `logTime` datetime NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `role_time_idx`(`roleID`, `logTime`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_ban_log
-- ----------------------------
DROP TABLE IF EXISTS `t_ban_log`;
CREATE TABLE `t_ban_log`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `zoneID` int(11) NOT NULL DEFAULT 0,
  `userID` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `roleID` int(11) NOT NULL DEFAULT 0,
  `banType` int(11) NOT NULL DEFAULT 0,
  `banID` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `banCount` int(11) NOT NULL DEFAULT 0,
  `logTime` datetime NULL,
  `deviceID` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `ban_idx`(`zoneID`, `userID`, `logTime`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_ban_trade
-- ----------------------------
DROP TABLE IF EXISTS `t_ban_trade`;
CREATE TABLE `t_ban_trade`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `day` date NOT NULL,
  `hour` tinyint(4) NOT NULL,
  `distinct_roles` int(11) NOT NULL DEFAULT 0,
  `market_times` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `market_in_price` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `market_out_price` int(11) NOT NULL DEFAULT 0,
  `trade_times` int(11) NOT NULL DEFAULT 0,
  `trade_in_price` int(11) NOT NULL DEFAULT 0,
  `trade_out_price` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`rid`, `day`, `hour`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_banggongbuy
-- ----------------------------
DROP TABLE IF EXISTS `t_banggongbuy`;
CREATE TABLE `t_banggongbuy`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `totalprice` int(11) NOT NULL DEFAULT 0,
  `leftbanggong` int(11) NOT NULL DEFAULT 0,
  `buytime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_banggonghist
-- ----------------------------
DROP TABLE IF EXISTS `t_banggonghist`;
CREATE TABLE `t_banggonghist`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `bhid` int(11) NOT NULL DEFAULT 0,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goods1num` int(11) NOT NULL DEFAULT 0,
  `goods2num` int(11) NOT NULL DEFAULT 0,
  `goods3num` int(11) NOT NULL DEFAULT 0,
  `goods4num` int(11) NOT NULL DEFAULT 0,
  `goods5num` int(11) NOT NULL DEFAULT 0,
  `tongqian` int(11) NOT NULL DEFAULT 0,
  `banggong` int(11) NOT NULL DEFAULT 0,
  `addtime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `rid_bhid`(`bhid`, `rid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 83 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_banghui
-- ----------------------------
DROP TABLE IF EXISTS `t_banghui`;
CREATE TABLE `t_banghui`  (
  `bhid` int(11) NOT NULL AUTO_INCREMENT,
  `bhname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `zoneid` int(11) NOT NULL DEFAULT 0,
  `rid` int(11) NOT NULL DEFAULT 0,
  `totalnum` int(11) NOT NULL DEFAULT 0,
  `totallevel` int(11) NOT NULL DEFAULT 0,
  `isverfiy` int(11) NOT NULL DEFAULT 0,
  `bhbulletin` char(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `buildtime` datetime NULL,
  `qiname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `qilevel` int(11) NOT NULL DEFAULT 1,
  `goods1num` int(11) NOT NULL DEFAULT 0,
  `goods2num` int(11) NOT NULL DEFAULT 0,
  `goods3num` int(11) NOT NULL DEFAULT 0,
  `goods4num` int(11) NOT NULL DEFAULT 0,
  `goods5num` int(11) NOT NULL DEFAULT 0,
  `tongqian` int(11) NOT NULL DEFAULT 0,
  `jitan` int(11) NOT NULL DEFAULT 1,
  `junxie` int(11) NOT NULL DEFAULT 1,
  `guanghuan` int(11) NOT NULL DEFAULT 1,
  `isdel` int(11) NOT NULL DEFAULT 0,
  `totalcombatforce` int(11) NOT NULL DEFAULT 0,
  `fubenid` int(11) NOT NULL DEFAULT 0,
  `fubenstate` tinyint(4) NOT NULL DEFAULT 0,
  `openday` smallint(6) NOT NULL DEFAULT 0,
  `killers` char(192) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `can_mod_name_times` int(11) NOT NULL DEFAULT 0,
  `zhengduoweek` int(11) NOT NULL DEFAULT 0,
  `zhengduousedtime` int(11) NOT NULL DEFAULT 0,
  `voice` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`bhid`) USING BTREE,
  UNIQUE INDEX `bhname_zoneid`(`bhname`, `zoneid`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 200000 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_banghui_match_support_flag
-- ----------------------------
DROP TABLE IF EXISTS `t_banghui_match_support_flag`;
CREATE TABLE `t_banghui_match_support_flag`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `season` int(11) NOT NULL DEFAULT 0,
  `round` tinyint(4) NOT NULL DEFAULT 0,
  `bhid1` int(11) NOT NULL DEFAULT 0,
  `bhid2` int(11) NOT NULL DEFAULT 0,
  `guess` tinyint(4) NOT NULL DEFAULT 0,
  `is_award` tinyint(4) NOT NULL DEFAULT 0,
  `time` datetime NULL,
  UNIQUE INDEX `key_r_s_r_b`(`rid`, `season`, `round`, `bhid1`, `bhid2`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_blackuserid
-- ----------------------------
DROP TABLE IF EXISTS `t_blackuserid`;
CREATE TABLE `t_blackuserid`  (
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`userid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_bocai_buy_history
-- ----------------------------
DROP TABLE IF EXISTS `t_bocai_buy_history`;
CREATE TABLE `t_bocai_buy_history`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `RoleName` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `ZoneID` int(11) NOT NULL DEFAULT 0,
  `UserID` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ServerID` int(11) NOT NULL DEFAULT 0,
  `BuyNum` int(11) NOT NULL DEFAULT 0,
  `BuyValue` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `IsSend` tinyint(4) NOT NULL DEFAULT 0,
  `IsWin` tinyint(4) NOT NULL DEFAULT 0,
  `BocaiType` tinyint(4) NOT NULL DEFAULT 0,
  `DataPeriods` bigint(20) NOT NULL DEFAULT 0,
  `UpdateTime` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`rid`, `BocaiType`, `DataPeriods`, `ServerID`, `BuyValue`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_bocai_open_lottery
-- ----------------------------
DROP TABLE IF EXISTS `t_bocai_open_lottery`;
CREATE TABLE `t_bocai_open_lottery`  (
  `DataPeriods` bigint(20) NOT NULL DEFAULT 0,
  `WinInfo` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `strWinNum` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `BocaiType` tinyint(4) NOT NULL DEFAULT 0,
  `SurplusBalance` bigint(20) NOT NULL DEFAULT 0,
  `AllBalance` bigint(20) NOT NULL DEFAULT 0,
  `XiaoHaoDaiBi` int(11) NOT NULL DEFAULT 0,
  `IsAward` tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`DataPeriods`, `BocaiType`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_bocai_shop
-- ----------------------------
DROP TABLE IF EXISTS `t_bocai_shop`;
CREATE TABLE `t_bocai_shop`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `ID` int(11) NOT NULL DEFAULT 0,
  `BuyNum` int(11) NOT NULL DEFAULT 0,
  `Periods` int(11) NOT NULL DEFAULT 0,
  `WuPinID` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`rid`, `ID`, `Periods`, `WuPinID`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_buffer
-- ----------------------------
DROP TABLE IF EXISTS `t_buffer`;
CREATE TABLE `t_buffer`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `bufferid` int(11) NOT NULL DEFAULT 0,
  `starttime` bigint(20) NOT NULL DEFAULT 0,
  `buffersecs` bigint(20) NOT NULL DEFAULT 0,
  `bufferval` bigint(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `rid_bufferid`(`rid`, `bufferid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 2690 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_building
-- ----------------------------
DROP TABLE IF EXISTS `t_building`;
CREATE TABLE `t_building`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `buildid` int(11) NOT NULL DEFAULT 0,
  `taskid_1` int(11) NOT NULL DEFAULT 0,
  `taskid_2` int(11) NOT NULL DEFAULT 0,
  `taskid_3` int(11) NOT NULL DEFAULT 0,
  `taskid_4` int(11) NOT NULL DEFAULT 0,
  `level` int(11) NOT NULL DEFAULT 0,
  `exp` int(11) NOT NULL DEFAULT 0,
  `developtime` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`rid`, `buildid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_building_log
-- ----------------------------
DROP TABLE IF EXISTS `t_building_log`;
CREATE TABLE `t_building_log`  (
  `time` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `task_role` int(11) NOT NULL DEFAULT 0,
  `task` int(11) NOT NULL DEFAULT 0,
  `refresh_role` int(11) NOT NULL DEFAULT 0,
  `refresh` int(11) NOT NULL DEFAULT 0,
  `open_role` int(11) NOT NULL DEFAULT 0,
  `open` int(11) NOT NULL DEFAULT 0,
  `push` int(11) NOT NULL DEFAULT 0,
  `pushuse` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`time`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_bulletin
-- ----------------------------
DROP TABLE IF EXISTS `t_bulletin`;
CREATE TABLE `t_bulletin`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `msgid` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `intervals` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bulletintext` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `fromdate` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `todate` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `opttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `msgid`(`msgid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 4 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_change_name
-- ----------------------------
DROP TABLE IF EXISTS `t_change_name`;
CREATE TABLE `t_change_name`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `roleid` int(11) NOT NULL,
  `oldname` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '',
  `newname` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT '',
  `type` tinyint(3) NOT NULL DEFAULT 0,
  `cost_diamond` int(11) NOT NULL DEFAULT 0,
  `time` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `key_roleid`(`roleid`) USING BTREE,
  INDEX `key_oldname`(`oldname`) USING BTREE,
  INDEX `key_newname`(`newname`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 7 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_change_name_banghui
-- ----------------------------
DROP TABLE IF EXISTS `t_change_name_banghui`;
CREATE TABLE `t_change_name_banghui`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bhid` int(11) NOT NULL,
  `by_role` int(11) NOT NULL,
  `old_name` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `new_name` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `time` datetime NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `key_old_name`(`old_name`) USING BTREE,
  INDEX `key_new_name`(`new_name`) USING BTREE,
  INDEX `key_bhid`(`bhid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_chengzhu
-- ----------------------------
DROP TABLE IF EXISTS `t_chengzhu`;
CREATE TABLE `t_chengzhu`  (
  `Id` int(11) NOT NULL DEFAULT 0,
  `bhid` int(11) NOT NULL DEFAULT 0,
  `kicknum` int(11) NOT NULL DEFAULT 0,
  `totaltax` int(11) NOT NULL DEFAULT 0,
  `taxdayid` int(11) NOT NULL DEFAULT 0,
  `todaytax` int(11) NOT NULL DEFAULT 0,
  `yestodaytax` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_cityinfo
-- ----------------------------
DROP TABLE IF EXISTS `t_cityinfo`;
CREATE TABLE `t_cityinfo`  (
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `dayid` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `region` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `cityname` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `onlinesecs` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `usedmoney` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `inputmoney` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `activeval` int(10) UNSIGNED NOT NULL DEFAULT 0,
  `lastip` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `starttime` datetime NULL,
  `logouttime` datetime NULL,
  UNIQUE INDEX `userid_dayid_cityname`(`userid`, `dayid`, `cityname`) USING BTREE,
  INDEX `starttime`(`starttime`) USING BTREE,
  INDEX `userid_cityname`(`userid`, `cityname`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_config
-- ----------------------------
DROP TABLE IF EXISTS `t_config`;
CREATE TABLE `t_config`  (
  `paramname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `paramvalue` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  UNIQUE INDEX `paramname`(`paramname`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_config
-- ----------------------------
INSERT INTO `t_config` VALUES ('anti-addiction', '0');
INSERT INTO `t_config` VALUES ('anti-addiction-hint', '10800');
INSERT INTO `t_config` VALUES ('anti-addiction-warning', '18000');
INSERT INTO `t_config` VALUES ('anti-addiction-restart', '18000');
INSERT INTO `t_config` VALUES ('big_award_id', '100');
INSERT INTO `t_config` VALUES ('songli_id', '1');
INSERT INTO `t_config` VALUES ('disable-speed-up', '1');
INSERT INTO `t_config` VALUES ('money-to-jifen', '1');
INSERT INTO `t_config` VALUES ('half_yinliang_period', '0');
INSERT INTO `t_config` VALUES ('speed-up-secs', '45');
INSERT INTO `t_config` VALUES ('low-nofall-level', '100');
INSERT INTO `t_config` VALUES ('up-nofall-level', '100');
INSERT INTO `t_config` VALUES ('force-add-shenfenzheng', '0');
INSERT INTO `t_config` VALUES ('ban-speed-up-minutes', '0');
INSERT INTO `t_config` VALUES ('move-speed-count', '10');
INSERT INTO `t_config` VALUES ('punish-speed-secs', '10');
INSERT INTO `t_config` VALUES ('keydigtreasure', '1');
INSERT INTO `t_config` VALUES ('hasshengxiaoguess', '1');
INSERT INTO `t_config` VALUES ('allowsubgold', '1');
INSERT INTO `t_config` VALUES ('kaifutime', '2019-02-18');
INSERT INTO `t_config` VALUES ('canfetchmailattachment', '1');
INSERT INTO `t_config` VALUES ('userbegintime', '2019-02-18');
INSERT INTO `t_config` VALUES ('buchangtime', '2019-02-18');
INSERT INTO `t_config` VALUES ('jieridaysnum', '999');
INSERT INTO `t_config` VALUES ('hefuwckingnum', '0');
INSERT INTO `t_config` VALUES ('hefutime', '2019-02-18');
INSERT INTO `t_config` VALUES ('hefuwcking', '600000');
INSERT INTO `t_config` VALUES ('hefuwckingdayid', '51');
INSERT INTO `t_config` VALUES ('platformtype', 'yyb');
INSERT INTO `t_config` VALUES ('kl_giftcode_u_r_l', 'api1.qmqj.xy.com/GetLipin/GetLipin.aspx');
INSERT INTO `t_config` VALUES ('yuedutime', '2019-02-18');
INSERT INTO `t_config` VALUES ('yueduchoujiangstartday', '2019-02-18');
INSERT INTO `t_config` VALUES ('yueduchoujiangdaysnum', '10');
INSERT INTO `t_config` VALUES ('jieristartday', '2019-05-30');
INSERT INTO `t_config` VALUES ('gamedb_version', '2018-02-26 19 13122');
INSERT INTO `t_config` VALUES ('gameserver_version', '2018-02-26_19_8.0.0.13237');
INSERT INTO `t_config` VALUES ('force-update', '1');
INSERT INTO `t_config` VALUES ('hint-appver', '20150110');
INSERT INTO `t_config` VALUES ('flag_t_roles_auto_increment', '200000');
INSERT INTO `t_config` VALUES ('ChongJiGiftList', '1,8,6,4,19');
INSERT INTO `t_config` VALUES ('PKKingRole', '200053');
INSERT INTO `t_config` VALUES ('PKKingPushMsgDayID', '148');
INSERT INTO `t_config` VALUES ('BattlePushMsgDayID', '148');
INSERT INTO `t_config` VALUES ('money-to-yuanbao', '10');
INSERT INTO `t_config` VALUES ('has_change_name_sys_for_merge', '1');
INSERT INTO `t_config` VALUES ('loginwebkey', '');
INSERT INTO `t_config` VALUES ('ipurl', 'no');
INSERT INTO `t_config` VALUES ('ptid', '0');
INSERT INTO `t_config` VALUES ('lipinma_v1', '-1');
INSERT INTO `t_config` VALUES ('DeleteRoleNeedTime', '120');
INSERT INTO `t_config` VALUES ('juntuanbanghuimax', '6');
INSERT INTO `t_config` VALUES ('bhmatch_goldjoin', '');
INSERT INTO `t_config` VALUES ('ZhongShenZhiShenRole', '48609580');
INSERT INTO `t_config` VALUES ('CoupleArenaFengHuo', '0,0');
INSERT INTO `t_config` VALUES ('everydayact', '2754,1211|2755,1411|2756,1608');
INSERT INTO `t_config` VALUES ('ZhengBaOpenedFlag', '1');
INSERT INTO `t_config` VALUES ('AngelTempleRole', 'ÈËÉú¾ÍÊÇÀË');
INSERT INTO `t_config` VALUES ('AngelTempleMonsterUpgradeNumber', '3243.24');
INSERT INTO `t_config` VALUES ('chat_world_level', '750');
INSERT INTO `t_config` VALUES ('+', '750');
INSERT INTO `t_config` VALUES ('chat_private_level', '750');
INSERT INTO `t_config` VALUES ('chat_near_level', '750');
INSERT INTO `t_config` VALUES ('vip_fullpurchase', '1312');
INSERT INTO `t_config` VALUES ('comp_monster_1', '0,0,63667533600000,10,1.000');
INSERT INTO `t_config` VALUES ('comp_monster_2', '0,0,63667533600000,10,1.000');
INSERT INTO `t_config` VALUES ('comp_monster_3', '0,0,63667533600000,10,1.000');
INSERT INTO `t_config` VALUES ('reborn_boss_96000', '9830000,2018-07-18 23$52$09');
INSERT INTO `t_config` VALUES ('ZorkAwardSeasonID', '20190520');
INSERT INTO `t_config` VALUES ('hefu_luolan_guildid', '600000,0|600000,0');
INSERT INTO `t_config` VALUES ('qinggongyan_roleid', '');
INSERT INTO `t_config` VALUES ('qinggongyan_guildname', '');
INSERT INTO `t_config` VALUES ('qinggongyan_startday', '');
INSERT INTO `t_config` VALUES ('qinggongyan_grade', '');
INSERT INTO `t_config` VALUES ('qinggongyan_joincount', '');
INSERT INTO `t_config` VALUES ('qinggongyan_joinmoney', '');
INSERT INTO `t_config` VALUES ('qinggongyan_jubanmoney', '');

-- ----------------------------
-- Table structure for t_consumelog
-- ----------------------------
DROP TABLE IF EXISTS `t_consumelog`;
CREATE TABLE `t_consumelog`  (
  `rid` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `cdate` date NOT NULL,
  PRIMARY KEY (`rid`, `cdate`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = ascii COLLATE = ascii_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_couple_arena_zhan_bao
-- ----------------------------
DROP TABLE IF EXISTS `t_couple_arena_zhan_bao`;
CREATE TABLE `t_couple_arena_zhan_bao`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `union_couple` bigint(20) NOT NULL DEFAULT 0,
  `man_rid` int(11) NOT NULL DEFAULT 0,
  `wife_rid` int(11) NOT NULL DEFAULT 0,
  `to_man_rid` int(11) NOT NULL DEFAULT 0,
  `to_man_zoneid` int(11) NOT NULL DEFAULT 0,
  `to_man_rname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `to_wife_rid` int(11) NOT NULL DEFAULT 0,
  `to_wife_zoneid` int(11) NOT NULL DEFAULT 0,
  `to_wife_rname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `is_win` tinyint(4) NOT NULL DEFAULT 0,
  `get_jifen` int(11) NOT NULL DEFAULT 0,
  `week` int(11) NOT NULL DEFAULT 0,
  `time` datetime NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `key_union_couple`(`union_couple`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_dailydata
-- ----------------------------
DROP TABLE IF EXISTS `t_dailydata`;
CREATE TABLE `t_dailydata`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `expdayid` int(11) NOT NULL DEFAULT 0,
  `todayexp` int(11) NOT NULL DEFAULT 0,
  `linglidayid` int(11) NOT NULL DEFAULT 0,
  `todaylingli` int(11) NOT NULL DEFAULT 0,
  `killbossdayid` int(11) NOT NULL DEFAULT 0,
  `todaykillboss` int(11) NOT NULL DEFAULT 0,
  `fubendayid` int(11) NOT NULL DEFAULT 0,
  `todayfubennum` int(11) NOT NULL DEFAULT 0,
  `wuxingdayid` int(11) NOT NULL DEFAULT 0,
  `wuxingnum` int(11) NOT NULL DEFAULT 0,
  `reborndayid` int(11) NOT NULL DEFAULT 0,
  `rebornexpmonster` int(11) NOT NULL DEFAULT 0,
  `rebornexpsale` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_dailyjingmai
-- ----------------------------
DROP TABLE IF EXISTS `t_dailyjingmai`;
CREATE TABLE `t_dailyjingmai`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `jmtime` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `jmnum` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_dailytasks
-- ----------------------------
DROP TABLE IF EXISTS `t_dailytasks`;
CREATE TABLE `t_dailytasks`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `huanid` int(11) NOT NULL DEFAULT 0,
  `rectime` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `recnum` int(11) NOT NULL DEFAULT 0,
  `taskClass` int(11) NOT NULL DEFAULT 0,
  `extdayid` int(11) NOT NULL DEFAULT 0,
  `extnum` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_taskClass`(`rid`, `taskClass`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_dayactivityinfo
-- ----------------------------
DROP TABLE IF EXISTS `t_dayactivityinfo`;
CREATE TABLE `t_dayactivityinfo`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `activityid` int(11) NOT NULL DEFAULT 0,
  `timeinfo` int(11) NOT NULL,
  `triggercount` int(11) NOT NULL DEFAULT 0,
  `totalpoint` bigint(20) NOT NULL DEFAULT 0,
  `lastgettime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  PRIMARY KEY (`roleid`, `activityid`) USING BTREE,
  UNIQUE INDEX `roleid_activity_timestr`(`roleid`, `activityid`, `timeinfo`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_djpoints
-- ----------------------------
DROP TABLE IF EXISTS `t_djpoints`;
CREATE TABLE `t_djpoints`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `djpoint` int(11) NOT NULL DEFAULT 0,
  `total` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `wincnt` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `yestoday` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `lastweek` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `lastmonth` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `dayupdown` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `weekupdown` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `monthupdown` int(11) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `rid`(`rid`) USING BTREE,
  INDEX `djpoint`(`djpoint`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_everyday_activity
-- ----------------------------
DROP TABLE IF EXISTS `t_everyday_activity`;
CREATE TABLE `t_everyday_activity`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `groupid` int(11) NOT NULL DEFAULT 0,
  `actid` int(11) NOT NULL DEFAULT 0,
  `purchaseNum` int(11) NOT NULL DEFAULT 0,
  `countNum` int(11) NOT NULL DEFAULT 0,
  `activeDay` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_groupid_actid`(`rid`, `groupid`, `actid`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_exchange1
-- ----------------------------
DROP TABLE IF EXISTS `t_exchange1`;
CREATE TABLE `t_exchange1`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `leftgoodsnum` int(11) NOT NULL DEFAULT 0,
  `otherroleid` int(11) NOT NULL DEFAULT 0,
  `result` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `rectime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 805 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_exchange2
-- ----------------------------
DROP TABLE IF EXISTS `t_exchange2`;
CREATE TABLE `t_exchange2`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `yinliang` int(11) NOT NULL DEFAULT 0,
  `leftyinliang` int(11) NOT NULL DEFAULT 0,
  `otherroleid` int(11) NOT NULL DEFAULT 0,
  `rectime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_exchange3
-- ----------------------------
DROP TABLE IF EXISTS `t_exchange3`;
CREATE TABLE `t_exchange3`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `yuanbao` int(11) NOT NULL DEFAULT 0,
  `leftyuanbao` int(11) NOT NULL DEFAULT 0,
  `otherroleid` int(11) NOT NULL DEFAULT 0,
  `rectime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 143 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_facebook
-- ----------------------------
DROP TABLE IF EXISTS `t_facebook`;
CREATE TABLE `t_facebook`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `serverID` int(11) NOT NULL,
  `uID` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `roleID` int(11) NOT NULL,
  `giftID` int(11) NOT NULL,
  `updatetime` datetime NULL,
  `state` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `updatetime+idx`(`updatetime`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_fallgoods
-- ----------------------------
DROP TABLE IF EXISTS `t_fallgoods`;
CREATE TABLE `t_fallgoods`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `autoid` int(11) NOT NULL DEFAULT 0,
  `goodsdbid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `binding` int(11) NOT NULL DEFAULT 0,
  `quality` int(11) NOT NULL DEFAULT 0,
  `forgelevel` int(11) NOT NULL DEFAULT 0,
  `jewellist` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `mapname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `goodsgrid` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `fromname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `rectime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 4829 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_firstcharge
-- ----------------------------
DROP TABLE IF EXISTS `t_firstcharge`;
CREATE TABLE `t_firstcharge`  (
  `uid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0',
  `charge_info` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `notget` int(32) NOT NULL,
  PRIMARY KEY (`uid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_fluorescent_gem_equip
-- ----------------------------
DROP TABLE IF EXISTS `t_fluorescent_gem_equip`;
CREATE TABLE `t_fluorescent_gem_equip`  (
  `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT,
  `roleid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `position` tinyint(4) NOT NULL DEFAULT 0,
  `type` tinyint(4) NOT NULL DEFAULT 0,
  `equiptime` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `bind` tinyint(4) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 2709 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_friends
-- ----------------------------
DROP TABLE IF EXISTS `t_friends`;
CREATE TABLE `t_friends`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `myid` int(11) NOT NULL DEFAULT 0,
  `otherid` int(11) NOT NULL DEFAULT 0,
  `friendType` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `unique_mo`(`myid`, `otherid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1004 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_fuben
-- ----------------------------
DROP TABLE IF EXISTS `t_fuben`;
CREATE TABLE `t_fuben`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `fubenid` int(11) NOT NULL DEFAULT 0,
  `dayid` int(11) NOT NULL DEFAULT 0,
  `enternum` int(11) NOT NULL DEFAULT 0,
  `quickpasstimer` int(11) NOT NULL DEFAULT 0,
  `finishnum` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_fubenid`(`rid`, `fubenid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_fubenhist
-- ----------------------------
DROP TABLE IF EXISTS `t_fubenhist`;
CREATE TABLE `t_fubenhist`  (
  `fubenid` int(11) NOT NULL DEFAULT 0,
  `rid` int(11) NOT NULL DEFAULT 0,
  `rname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `usedsecs` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `fubenid`(`fubenid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_fund
-- ----------------------------
DROP TABLE IF EXISTS `t_fund`;
CREATE TABLE `t_fund`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `zoneID` int(11) NOT NULL,
  `userID` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `roleID` int(11) NOT NULL,
  `fundType` int(11) NOT NULL DEFAULT 0,
  `fundID` int(11) NOT NULL DEFAULT 0,
  `buyTime` datetime NULL,
  `awardID` int(11) NOT NULL DEFAULT 0,
  `value1` int(11) NOT NULL DEFAULT 0,
  `value2` int(11) NOT NULL DEFAULT 0,
  `state` int(11) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 164 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_fuwen
-- ----------------------------
DROP TABLE IF EXISTS `t_fuwen`;
CREATE TABLE `t_fuwen`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `tabid` int(11) NOT NULL DEFAULT 0,
  `name` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '*',
  `fuwenequip` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `shenshiactive` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `skillequip` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_tab`(`rid`, `tabid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_giftcode
-- ----------------------------
DROP TABLE IF EXISTS `t_giftcode`;
CREATE TABLE `t_giftcode`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userid` char(64) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL DEFAULT '0',
  `rid` int(11) NOT NULL DEFAULT 0,
  `giftid` varchar(30) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT NULL,
  `codeno` char(8) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT NULL,
  `usetime` datetime NULL DEFAULT NULL,
  `mailid` tinyint(1) NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `_codeno_idx`(`codeno`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = ascii COLLATE = ascii_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_givemoney
-- ----------------------------
DROP TABLE IF EXISTS `t_givemoney`;
CREATE TABLE `t_givemoney`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `yuanbao` int(11) NOT NULL DEFAULT 0,
  `rectime` datetime NULL,
  `givetype` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_gmmsg
-- ----------------------------
DROP TABLE IF EXISTS `t_gmmsg`;
CREATE TABLE `t_gmmsg`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `msg` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 22118 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_gold_auction
-- ----------------------------
DROP TABLE IF EXISTS `t_gold_auction`;
CREATE TABLE `t_gold_auction`  (
  `BuyerData` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `AuctionTime` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `AuctionType` tinyint(4) NOT NULL DEFAULT 0,
  `AuctionSource` tinyint(4) NOT NULL DEFAULT 0,
  `ProductionTime` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `StrGoods` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `BossLife` bigint(20) NOT NULL DEFAULT 0,
  `KillBossRoleID` int(11) NOT NULL DEFAULT 0,
  `UpDBWay` char(6) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0',
  `UpdateTime` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `AttackerList` longblob NOT NULL,
  PRIMARY KEY (`ProductionTime`, `AuctionSource`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_goldbuy
-- ----------------------------
DROP TABLE IF EXISTS `t_goldbuy`;
CREATE TABLE `t_goldbuy`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `totalprice` int(11) NOT NULL DEFAULT 0,
  `leftgold` int(11) NOT NULL DEFAULT 0,
  `buytime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 9180 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_goods
-- ----------------------------
DROP TABLE IF EXISTS `t_goods`;
CREATE TABLE `t_goods`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `isusing` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `forge_level` int(11) UNSIGNED NOT NULL DEFAULT 1,
  `starttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `endtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `site` int(11) NOT NULL DEFAULT 0,
  `quality` int(11) UNSIGNED NOT NULL DEFAULT 1,
  `Props` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `gcount` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `origholenum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `rmbholenum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `jewellist` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `binding` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bagindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `salemoney1` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `saleyuanbao` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `saleyinpiao` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `addpropindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bornindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `lucky` int(11) NOT NULL DEFAULT 0,
  `strong` int(11) NOT NULL DEFAULT 0,
  `excellenceinfo` int(11) NOT NULL DEFAULT 0,
  `appendproplev` int(11) NOT NULL DEFAULT 0,
  `equipchangelife` int(11) NOT NULL DEFAULT 0,
  `washprops` varchar(256) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `ehinfo` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `juhun` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 716010 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_goods_bak
-- ----------------------------
DROP TABLE IF EXISTS `t_goods_bak`;
CREATE TABLE `t_goods_bak`  (
  `Id` int(11) NOT NULL,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `isusing` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `forge_level` int(11) UNSIGNED NOT NULL DEFAULT 1,
  `starttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `endtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `site` int(11) NOT NULL DEFAULT 0,
  `quality` int(11) UNSIGNED NOT NULL DEFAULT 1,
  `Props` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `gcount` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `origholenum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `rmbholenum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `jewellist` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `binding` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bagindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `salemoney1` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `saleyuanbao` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `saleyinpiao` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `addpropindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bornindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `lucky` int(11) NOT NULL DEFAULT 0,
  `strong` int(11) NOT NULL DEFAULT 0,
  `excellenceinfo` int(11) NOT NULL DEFAULT 0,
  `appendproplev` int(11) NOT NULL DEFAULT 0,
  `equipchangelife` int(11) NOT NULL DEFAULT 0,
  `washprops` varchar(256) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `ehinfo` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `juhun` int(11) NOT NULL DEFAULT 0,
  `opstate` int(11) NOT NULL DEFAULT 0,
  `optime` datetime NULL,
  `oprole` int(11) NOT NULL DEFAULT 0,
  INDEX `idx_id`(`Id`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_goods_bak_1
-- ----------------------------
DROP TABLE IF EXISTS `t_goods_bak_1`;
CREATE TABLE `t_goods_bak_1`  (
  `Id` int(11) NOT NULL,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `isusing` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `forge_level` int(11) UNSIGNED NOT NULL DEFAULT 1,
  `starttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `endtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `site` int(11) NOT NULL DEFAULT 0,
  `quality` int(11) UNSIGNED NOT NULL DEFAULT 1,
  `Props` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `gcount` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `origholenum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `rmbholenum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `jewellist` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `binding` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bagindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `salemoney1` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `saleyuanbao` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `saleyinpiao` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `addpropindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bornindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `lucky` int(11) NOT NULL DEFAULT 0,
  `strong` int(11) NOT NULL DEFAULT 0,
  `excellenceinfo` int(11) NOT NULL DEFAULT 0,
  `appendproplev` int(11) NOT NULL DEFAULT 0,
  `equipchangelife` int(11) NOT NULL DEFAULT 0,
  `washprops` varchar(256) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `ehinfo` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `juhun` int(11) NOT NULL DEFAULT 0,
  `opstate` int(11) NOT NULL DEFAULT 0,
  `optime` datetime NULL,
  `oprole` int(11) NOT NULL DEFAULT 0,
  INDEX `idx_id`(`Id`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_goodslimit
-- ----------------------------
DROP TABLE IF EXISTS `t_goodslimit`;
CREATE TABLE `t_goodslimit`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `dayid` int(11) NOT NULL DEFAULT 0,
  `usednum` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_goodsid`(`rid`, `goodsid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_goodsprops
-- ----------------------------
DROP TABLE IF EXISTS `t_goodsprops`;
CREATE TABLE `t_goodsprops`  (
  `id` int(10) NOT NULL,
  `rid` int(10) NOT NULL,
  `type` int(10) NOT NULL,
  `props` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `isdel` int(10) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`, `rid`, `type`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_groupmail
-- ----------------------------
DROP TABLE IF EXISTS `t_groupmail`;
CREATE TABLE `t_groupmail`  (
  `gmailid` int(11) NOT NULL,
  `subject` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `content` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `conditions` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `inputtime` datetime NULL,
  `endtime` datetime NULL,
  `yinliang` int(11) NOT NULL DEFAULT 0,
  `tongqian` int(11) NOT NULL DEFAULT 0,
  `yuanbao` int(11) NOT NULL DEFAULT 0,
  `goodlist` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`gmailid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_guard_soul
-- ----------------------------
DROP TABLE IF EXISTS `t_guard_soul`;
CREATE TABLE `t_guard_soul`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `soul_type` int(11) NOT NULL DEFAULT 0,
  `equip_slot` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`roleid`, `soul_type`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_guard_statue
-- ----------------------------
DROP TABLE IF EXISTS `t_guard_statue`;
CREATE TABLE `t_guard_statue`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `slot_cnt` int(11) NOT NULL DEFAULT 0,
  `level` int(11) NOT NULL DEFAULT 0,
  `suit` int(11) NOT NULL DEFAULT 0,
  `total_guard_point` int(11) NOT NULL DEFAULT 0,
  `lastday_recover_point` int(11) NOT NULL DEFAULT 0,
  `lastday_recover_offset` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`roleid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_hdtequan
-- ----------------------------
DROP TABLE IF EXISTS `t_hdtequan`;
CREATE TABLE `t_hdtequan`  (
  `Id` int(11) NOT NULL DEFAULT 0,
  `tolaofangdayid` int(11) NOT NULL DEFAULT 0,
  `tolaofangnum` int(11) NOT NULL DEFAULT 0,
  `offlaofangdayid` int(11) NOT NULL DEFAULT 0,
  `offlaofangnum` int(11) NOT NULL DEFAULT 0,
  `bancatdayid` int(11) NOT NULL DEFAULT 0,
  `bancatnum` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_holyitem
-- ----------------------------
DROP TABLE IF EXISTS `t_holyitem`;
CREATE TABLE `t_holyitem`  (
  `roleid` int(11) NOT NULL,
  `shengwu_type` tinyint(1) NOT NULL,
  `part_slot` tinyint(1) NOT NULL DEFAULT 0,
  `part_suit` tinyint(1) NULL DEFAULT NULL,
  `part_slice` int(11) NULL DEFAULT NULL,
  `fail_count` tinyint(3) NULL DEFAULT NULL,
  PRIMARY KEY (`roleid`, `shengwu_type`, `part_slot`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_hongbao_jieri_recv
-- ----------------------------
DROP TABLE IF EXISTS `t_hongbao_jieri_recv`;
CREATE TABLE `t_hongbao_jieri_recv`  (
  `keystr` char(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `rid` int(11) NOT NULL,
  `count` int(11) NOT NULL,
  `getawardtimes` int(11) NOT NULL,
  `lasttime` datetime NULL,
  `rname` char(40) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `zuanshi` bigint(20) NOT NULL DEFAULT 0,
  PRIMARY KEY (`keystr`, `rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_bin ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_hongbao_jieri_send
-- ----------------------------
DROP TABLE IF EXISTS `t_hongbao_jieri_send`;
CREATE TABLE `t_hongbao_jieri_send`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `keystr` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `senderid` int(11) NOT NULL,
  `sendtime` datetime NULL,
  `endtime` datetime NULL,
  `msg` char(64) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `zuanshi` int(11) NOT NULL,
  `type` tinyint(4) NOT NULL,
  `leftzuanshi` int(11) NOT NULL,
  `state` tinyint(4) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `keystr`(`keystr`, `senderid`) USING BTREE,
  INDEX `state`(`state`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_hongbao_recv
-- ----------------------------
DROP TABLE IF EXISTS `t_hongbao_recv`;
CREATE TABLE `t_hongbao_recv`  (
  `hongbaoid` int(11) NOT NULL,
  `rid` int(11) NOT NULL,
  `bhid` int(11) NOT NULL,
  `zuanshi` int(11) NOT NULL,
  `recvtime` datetime NULL,
  `rname` char(32) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`hongbaoid`, `rid`) USING BTREE,
  INDEX `bhid`(`bhid`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE,
  INDEX `recvtime`(`recvtime`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_bin ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_hongbao_send
-- ----------------------------
DROP TABLE IF EXISTS `t_hongbao_send`;
CREATE TABLE `t_hongbao_send`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `bhid` int(11) NOT NULL,
  `senderid` int(11) NOT NULL,
  `sendername` varchar(48) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `sendtime` datetime NULL,
  `endtime` datetime NULL,
  `msg` varchar(250) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `zuanshi` int(11) NOT NULL,
  `count` int(11) NOT NULL,
  `type` tinyint(4) NOT NULL,
  `leftzuanshi` int(11) NOT NULL,
  `leftcount` int(11) NOT NULL,
  `state` tinyint(4) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `senderid`(`senderid`) USING BTREE,
  INDEX `sendtime`(`sendtime`) USING BTREE,
  INDEX `state`(`state`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_horses
-- ----------------------------
DROP TABLE IF EXISTS `t_horses`;
CREATE TABLE `t_horses`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `horseid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bodyid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `propsNum` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `PropsVal` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `addtime` datetime NULL,
  `isdel` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `failednum` int(11) NOT NULL DEFAULT 0,
  `temptime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `tempnum` int(11) NOT NULL DEFAULT 0,
  `faileddayid` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_huodong
-- ----------------------------
DROP TABLE IF EXISTS `t_huodong`;
CREATE TABLE `t_huodong`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `loginweekid` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `logindayid` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `loginnum` int(11) NOT NULL DEFAULT 0,
  `newstep` int(11) NOT NULL DEFAULT 0,
  `steptime` datetime NULL,
  `lastmtime` int(11) NOT NULL DEFAULT 0,
  `curmid` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `curmtime` int(11) NOT NULL DEFAULT 0,
  `songliid` int(11) NOT NULL DEFAULT 0,
  `logingiftstate` int(11) NOT NULL DEFAULT 0,
  `onlinegiftstate` int(11) NOT NULL DEFAULT 0,
  `lastlimittimehuodongid` int(11) NOT NULL DEFAULT 0,
  `lastlimittimedayid` int(11) NOT NULL DEFAULT 0,
  `limittimeloginnum` int(11) NOT NULL DEFAULT 0,
  `limittimegiftstate` int(11) NOT NULL DEFAULT 0,
  `everydayonlineawardstep` int(11) NOT NULL DEFAULT 0,
  `geteverydayonlineawarddayid` int(11) NOT NULL DEFAULT 0,
  `serieslogingetawardstep` int(11) NOT NULL DEFAULT 0,
  `seriesloginawarddayid` int(11) NOT NULL DEFAULT 0,
  `seriesloginawardgoodsid` char(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `everydayonlineawardgoodsid` char(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  UNIQUE INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_huodongawardrolehist
-- ----------------------------
DROP TABLE IF EXISTS `t_huodongawardrolehist`;
CREATE TABLE `t_huodongawardrolehist`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `zoneid` int(11) UNSIGNED NOT NULL DEFAULT 1,
  `activitytype` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `keystr` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `hasgettimes` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `lastgettime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  INDEX `idx_rid_activitytype_keystr`(`rid`, `activitytype`, `keystr`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_huodongawarduserhist
-- ----------------------------
DROP TABLE IF EXISTS `t_huodongawarduserhist`;
CREATE TABLE `t_huodongawarduserhist`  (
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0',
  `activitytype` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `keystr` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `hasgettimes` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `lastgettime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  UNIQUE INDEX `uactkey`(`userid`, `activitytype`, `keystr`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_huodongawarduserhist_regress
-- ----------------------------
DROP TABLE IF EXISTS `t_huodongawarduserhist_regress`;
CREATE TABLE `t_huodongawarduserhist_regress`  (
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0',
  `activitytype` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `keystr` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `hasgettimes` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `lastgettime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `activitydata` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `active_stage` char(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  UNIQUE INDEX `uactkey`(`activitytype`, `userid`, `keystr`, `active_stage`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_huodongpaihang
-- ----------------------------
DROP TABLE IF EXISTS `t_huodongpaihang`;
CREATE TABLE `t_huodongpaihang`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `rname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `zoneid` int(11) NOT NULL DEFAULT 0,
  `type` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `paihang` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `phvalue` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `paihangtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  INDEX `rid_idx`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_inputhist
-- ----------------------------
DROP TABLE IF EXISTS `t_inputhist`;
CREATE TABLE `t_inputhist`  (
  `Id` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `lastid` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_inputlog
-- ----------------------------
DROP TABLE IF EXISTS `t_inputlog`;
CREATE TABLE `t_inputlog`  (
  `Id` int(11) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  `amount` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `u` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0',
  `rid` int(11) NOT NULL DEFAULT 0,
  `order_no` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `cporder_no` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `time` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `sign` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `inputtime` datetime NULL,
  `result` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `zoneid` int(11) NOT NULL DEFAULT 0,
  `itemid` int(11) NOT NULL DEFAULT 0,
  `chargetime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `inputtime`(`inputtime`) USING BTREE,
  INDEX `query_money`(`inputtime`, `u`, `zoneid`, `result`) USING BTREE,
  INDEX `uid`(`u`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 307 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_inputlog2
-- ----------------------------
DROP TABLE IF EXISTS `t_inputlog2`;
CREATE TABLE `t_inputlog2`  (
  `Id` int(11) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT,
  `amount` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `u` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0',
  `rid` int(11) NOT NULL DEFAULT 0,
  `order_no` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `cporder_no` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `time` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `sign` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `inputtime` datetime NULL,
  `result` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `zoneid` int(11) NOT NULL DEFAULT 0,
  `itemid` int(11) NOT NULL DEFAULT 0,
  `chargetime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `inputtime`(`inputtime`) USING BTREE,
  INDEX `query_money`(`inputtime`, `u`, `zoneid`, `result`) USING BTREE,
  INDEX `uid`(`u`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_jierizengsong
-- ----------------------------
DROP TABLE IF EXISTS `t_jierizengsong`;
CREATE TABLE `t_jierizengsong`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `sender` int(11) NOT NULL DEFAULT 0,
  `receiver` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodscnt` int(11) NOT NULL DEFAULT 0,
  `sendtime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `key_sendtime`(`sendtime`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 94 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_jingjichang
-- ----------------------------
DROP TABLE IF EXISTS `t_jingjichang`;
CREATE TABLE `t_jingjichang`  (
  `roleId` int(11) NOT NULL,
  `roleName` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `name` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `zoneId` int(11) NOT NULL,
  `level` int(11) NOT NULL,
  `changeLiveCount` int(11) NOT NULL,
  `occupationId` int(11) NOT NULL,
  `winCount` int(11) NOT NULL DEFAULT 0,
  `ranking` int(11) NOT NULL DEFAULT -1,
  `nextRewardTime` bigint(20) NOT NULL DEFAULT 0,
  `nextChallengeTime` bigint(20) NOT NULL DEFAULT 0,
  `version` int(11) NOT NULL DEFAULT 0,
  `baseProps` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `extProps` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `equipDatas` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `skillDatas` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `CombatForce` int(11) NOT NULL DEFAULT 0,
  `sex` tinyint(11) NOT NULL,
  `wingData` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `settingFlags` bigint(20) NOT NULL DEFAULT 0,
  `maxwincnt` int(11) NULL DEFAULT 0,
  `shenshiequip` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `passiveEffect` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `suboccupation` int(11) NULL DEFAULT 0,
  PRIMARY KEY (`roleId`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_jingjichang_zhanbao
-- ----------------------------
DROP TABLE IF EXISTS `t_jingjichang_zhanbao`;
CREATE TABLE `t_jingjichang_zhanbao`  (
  `pkId` int(11) NOT NULL AUTO_INCREMENT,
  `roleId` int(11) NOT NULL,
  `zhanbaoType` int(11) NOT NULL,
  `challengeName` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `value` int(11) NOT NULL,
  `createTime` datetime NULL,
  PRIMARY KEY (`pkId`) USING BTREE,
  INDEX `idx_t_jingjichang_zhanbao_roleId`(`roleId`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 724 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_jingmai
-- ----------------------------
DROP TABLE IF EXISTS `t_jingmai`;
CREATE TABLE `t_jingmai`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `jmid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `jmlevel` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bodylevel` int(11) UNSIGNED NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `rid_jmid_bl`(`rid`, `jmid`, `bodylevel`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_juexing
-- ----------------------------
DROP TABLE IF EXISTS `t_juexing`;
CREATE TABLE `t_juexing`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `suitid` int(11) NOT NULL DEFAULT 0,
  `activite` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '*',
  UNIQUE INDEX `rid_suit`(`rid`, `suitid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_juexing_jlys
-- ----------------------------
DROP TABLE IF EXISTS `t_juexing_jlys`;
CREATE TABLE `t_juexing_jlys`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `activetype` int(11) NOT NULL DEFAULT 0,
  `activeids` char(48) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT ''
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_kf_5v5_zhandui
-- ----------------------------
DROP TABLE IF EXISTS `t_kf_5v5_zhandui`;
CREATE TABLE `t_kf_5v5_zhandui`  (
  `zhanduiid` int(10) NOT NULL AUTO_INCREMENT,
  `zhanduiname` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `xuanyan` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `leaderid` int(10) NOT NULL,
  `leaderrolename` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `zoneid` int(10) NOT NULL,
  `duanweiid` tinyint(3) NOT NULL DEFAULT 0,
  `duanweijifen` int(11) NOT NULL DEFAULT 0,
  `duanweirank` int(11) NOT NULL DEFAULT 0,
  `liansheng` smallint(5) NOT NULL DEFAULT 0,
  `fightcount` smallint(5) NOT NULL DEFAULT 0,
  `successcount` smallint(5) NOT NULL DEFAULT 0,
  `lastfighttime` datetime NOT NULL DEFAULT '2011-11-11 00:00:00',
  `monthduanweirank` int(10) NOT NULL DEFAULT 0,
  `zhanli` bigint(10) NOT NULL DEFAULT 0,
  `zorkjifen` int(11) NOT NULL DEFAULT 0,
  `zorkwin` int(11) NOT NULL DEFAULT 0,
  `zorkwinstreak` int(11) NOT NULL DEFAULT 0,
  `zorkbossinjure` int(11) NOT NULL DEFAULT 0,
  `zorklastfighttime` datetime NOT NULL DEFAULT '2011-11-11 00:00:00',
  `escapejifen` int(11) NOT NULL DEFAULT 0,
  `escapelastfighttime` datetime NOT NULL DEFAULT '2011-11-11 00:00:00',
  `data1` longblob NULL,
  `data2` longblob NULL,
  PRIMARY KEY (`zhanduiid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 2265 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_kf_day_role_log
-- ----------------------------
DROP TABLE IF EXISTS `t_kf_day_role_log`;
CREATE TABLE `t_kf_day_role_log`  (
  `gametype` tinyint(4) NOT NULL,
  `day` date NOT NULL,
  `rid` int(11) NOT NULL,
  `zoneid` int(11) NULL DEFAULT 0,
  `signup_count` smallint(6) NULL DEFAULT 0,
  `start_game_count` smallint(6) NULL DEFAULT 0,
  `success_count` smallint(6) NULL DEFAULT 0,
  `faild_count` smallint(6) NULL DEFAULT 0,
  PRIMARY KEY (`day`, `gametype`, `rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_kf_hysy_role_log
-- ----------------------------
DROP TABLE IF EXISTS `t_kf_hysy_role_log`;
CREATE TABLE `t_kf_hysy_role_log`  (
  `rid` int(11) NOT NULL,
  `day` date NOT NULL,
  `zoneid` int(11) NULL DEFAULT 0,
  `signup_count` smallint(6) NULL DEFAULT 0,
  `start_game_count` smallint(6) NULL DEFAULT 0,
  `success_count` smallint(6) NULL DEFAULT 0,
  `faild_count` smallint(6) NULL DEFAULT 0,
  PRIMARY KEY (`rid`, `day`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_kf_tianti_game_log
-- ----------------------------
DROP TABLE IF EXISTS `t_kf_tianti_game_log`;
CREATE TABLE `t_kf_tianti_game_log`  (
  `rid` int(11) NOT NULL,
  `zoneid1` int(11) NOT NULL,
  `rolename1` char(32) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `zoneid2` int(11) NOT NULL,
  `rolename2` char(32) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `success` tinyint(4) NOT NULL,
  `duanweijifenaward` int(11) NOT NULL,
  `rongyaoaward` int(11) NOT NULL,
  `endtime` datetime NOT NULL DEFAULT '2001-11-11 00:00:00',
  INDEX `idx_rid_endtime`(`rid`, `endtime`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_bin ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_kf_tianti_role
-- ----------------------------
DROP TABLE IF EXISTS `t_kf_tianti_role`;
CREATE TABLE `t_kf_tianti_role`  (
  `rid` int(10) UNSIGNED NOT NULL,
  `duanweiid` tinyint(3) UNSIGNED NOT NULL,
  `duanweijifen` int(5) UNSIGNED NOT NULL,
  `duanweirank` int(10) UNSIGNED NOT NULL,
  `liansheng` smallint(5) UNSIGNED NOT NULL,
  `fightcount` smallint(5) UNSIGNED NOT NULL,
  `successcount` smallint(5) UNSIGNED NOT NULL,
  `todayfightcount` smallint(5) UNSIGNED NOT NULL,
  `lastfightdayid` smallint(5) UNSIGNED NOT NULL,
  `monthduanweirank` int(10) UNSIGNED NOT NULL,
  `fetchmonthawarddate` date NOT NULL DEFAULT '2001-11-11',
  `rongyao` int(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci DELAY_KEY_WRITE = 1 ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_kfonlineawards
-- ----------------------------
DROP TABLE IF EXISTS `t_kfonlineawards`;
CREATE TABLE `t_kfonlineawards`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `dayid` int(11) NOT NULL DEFAULT 0,
  `yuanbao` int(11) NOT NULL DEFAULT 0,
  `totalrolenum` int(11) NOT NULL DEFAULT 0,
  `zoneid` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `dayid_zoneid`(`dayid`, `zoneid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_king_role_data
-- ----------------------------
DROP TABLE IF EXISTS `t_king_role_data`;
CREATE TABLE `t_king_role_data`  (
  `king_type` int(11) NOT NULL DEFAULT 0,
  `role_id` int(11) NOT NULL DEFAULT 0,
  `mod_time` datetime NULL,
  `roledata_ex` mediumtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`king_type`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_limit_usergoodsbuy
-- ----------------------------
DROP TABLE IF EXISTS `t_limit_usergoodsbuy`;
CREATE TABLE `t_limit_usergoodsbuy`  (
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `goodsid` int(11) NOT NULL,
  `dayid` int(11) NOT NULL,
  `usednum` int(11) NOT NULL,
  `active_stage` char(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  UNIQUE INDEX `userid_goodsid_stage`(`userid`, `goodsid`, `active_stage`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_limitgoodsbuy
-- ----------------------------
DROP TABLE IF EXISTS `t_limitgoodsbuy`;
CREATE TABLE `t_limitgoodsbuy`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `dayid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `usednum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_goodsid`(`rid`, `goodsid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_lingdi
-- ----------------------------
DROP TABLE IF EXISTS `t_lingdi`;
CREATE TABLE `t_lingdi`  (
  `lingdi` int(11) NOT NULL DEFAULT 0,
  `bhid` int(11) NOT NULL DEFAULT 0,
  `tax` int(11) NOT NULL DEFAULT 0,
  `takedayid` int(11) NOT NULL DEFAULT 0,
  `takedaynum` int(11) NOT NULL DEFAULT 0,
  `yestodaytax` int(11) NOT NULL DEFAULT 0,
  `taxdayid` int(11) NOT NULL DEFAULT 0,
  `todaytax` int(11) NOT NULL DEFAULT 0,
  `totaltax` int(11) NOT NULL DEFAULT 0,
  `awardfetchday` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `warrequest` tinytext CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  UNIQUE INDEX `lingdi`(`lingdi`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_lingdi
-- ----------------------------
INSERT INTO `t_lingdi` VALUES (7, 200000, 0, 0, 0, 0, 0, 0, 0, 0, '');

-- ----------------------------
-- Table structure for t_lingyu
-- ----------------------------
DROP TABLE IF EXISTS `t_lingyu`;
CREATE TABLE `t_lingyu`  (
  `roleid` int(11) NOT NULL,
  `type` int(11) NOT NULL,
  `level` int(6) NOT NULL DEFAULT 0,
  `suit` int(6) NOT NULL DEFAULT 0,
  PRIMARY KEY (`roleid`, `type`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_linpinma
-- ----------------------------
DROP TABLE IF EXISTS `t_linpinma`;
CREATE TABLE `t_linpinma`  (
  `lipinma` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `huodongid` int(11) NOT NULL DEFAULT 0,
  `maxnum` int(11) NOT NULL DEFAULT 0,
  `usednum` int(11) NOT NULL DEFAULT 0,
  `ptid` int(11) NOT NULL DEFAULT 0,
  `ptrepeat` int(11) UNSIGNED NOT NULL DEFAULT 0,
  UNIQUE INDEX `lipinma`(`lipinma`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_login
-- ----------------------------
DROP TABLE IF EXISTS `t_login`;
CREATE TABLE `t_login`  (
  `userid` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `dayid` int(11) NULL DEFAULT 0,
  `rid` bigint(11) NOT NULL,
  `logintime` datetime NULL DEFAULT NULL,
  `logouttime` datetime NULL DEFAULT NULL,
  `ip` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `mac` varchar(30) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `zoneid` mediumint(8) NULL DEFAULT NULL,
  `onlinesecs` mediumint(8) NULL DEFAULT 0,
  `loginnum` mediumint(8) NULL DEFAULT 0,
  `c1` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `c2` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  UNIQUE INDEX `userid_dayid_ip`(`userid`, `dayid`, `ip`) USING BTREE,
  INDEX `logintime`(`logintime`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_mail
-- ----------------------------
DROP TABLE IF EXISTS `t_mail`;
CREATE TABLE `t_mail`  (
  `mailid` int(11) NOT NULL AUTO_INCREMENT,
  `senderrid` int(11) NOT NULL DEFAULT 0,
  `senderrname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `sendtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `receiverrid` int(11) NOT NULL DEFAULT 0,
  `reveiverrname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `readtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `isread` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `mailtype` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `hasfetchattachment` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `subject` varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `content` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `yinliang` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `tongqian` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `yuanbao` int(11) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`mailid`) USING BTREE,
  INDEX `receiverrid`(`receiverrid`) USING BTREE,
  INDEX `senderrid_idx`(`senderrid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 200000 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_mail_fumo_map
-- ----------------------------
DROP TABLE IF EXISTS `t_mail_fumo_map`;
CREATE TABLE `t_mail_fumo_map`  (
  `tid` int(11) NOT NULL,
  `senderid` int(11) NOT NULL,
  `recid_list` varchar(500) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL,
  `accept` int(11) NOT NULL DEFAULT 0,
  `give` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`tid`, `senderid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_mailfumo
-- ----------------------------
DROP TABLE IF EXISTS `t_mailfumo`;
CREATE TABLE `t_mailfumo`  (
  `maillid` int(11) NOT NULL AUTO_INCREMENT,
  `senderrid` int(11) NOT NULL DEFAULT 0,
  `senderrname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `senderjob` int(11) NOT NULL DEFAULT 0,
  `sendtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `receiverrid` int(11) NOT NULL DEFAULT 0,
  `isread` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `readtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `fumomoney` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `content` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`maillid`) USING BTREE,
  INDEX `receiverrid`(`receiverrid`) USING BTREE,
  INDEX `senderrid_idx`(`senderrid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 199 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_mailgoods
-- ----------------------------
DROP TABLE IF EXISTS `t_mailgoods`;
CREATE TABLE `t_mailgoods`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `mailid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `forge_level` int(11) UNSIGNED NOT NULL DEFAULT 1,
  `quality` int(11) UNSIGNED NOT NULL DEFAULT 1,
  `Props` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `gcount` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `binding` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `origholenum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `rmbholenum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `jewellist` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `addpropindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bornindex` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `lucky` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `strong` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `excellenceinfo` int(11) NOT NULL DEFAULT 0,
  `appendproplev` int(11) NOT NULL DEFAULT 0,
  `equipchangelife` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `mailid`(`mailid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 12403 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_mailtemp
-- ----------------------------
DROP TABLE IF EXISTS `t_mailtemp`;
CREATE TABLE `t_mailtemp`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `mailid` int(11) NOT NULL DEFAULT 0,
  `receiverrid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 2983 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_mallbuy
-- ----------------------------
DROP TABLE IF EXISTS `t_mallbuy`;
CREATE TABLE `t_mallbuy`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `totalprice` int(11) NOT NULL DEFAULT 0,
  `leftmoney` int(11) NOT NULL DEFAULT 0,
  `buytime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 5861 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_marry
-- ----------------------------
DROP TABLE IF EXISTS `t_marry`;
CREATE TABLE `t_marry`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `spouseid` int(11) NULL DEFAULT -1,
  `marrytype` int(6) NULL DEFAULT NULL,
  `ringid` int(11) NULL DEFAULT NULL,
  `goodwillexp` int(11) NULL DEFAULT NULL,
  `goodwillstar` int(6) NULL DEFAULT NULL,
  `goodwilllevel` int(6) NULL DEFAULT NULL,
  `givenrose` int(6) NULL DEFAULT NULL,
  `lovemessage` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `autoreject` int(6) NULL DEFAULT NULL,
  `changtime` datetime NULL,
  PRIMARY KEY (`roleid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_marryparty
-- ----------------------------
DROP TABLE IF EXISTS `t_marryparty`;
CREATE TABLE `t_marryparty`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `partytype` int(6) NULL DEFAULT NULL,
  `joincount` int(6) NULL DEFAULT NULL,
  `starttime` datetime NULL DEFAULT NULL,
  `husbandid` int(11) NOT NULL DEFAULT 0,
  `wifeid` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`roleid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_marryparty_join
-- ----------------------------
DROP TABLE IF EXISTS `t_marryparty_join`;
CREATE TABLE `t_marryparty_join`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `partyroleid` int(11) NOT NULL DEFAULT 0,
  `joincount` int(6) NULL DEFAULT NULL,
  PRIMARY KEY (`roleid`, `partyroleid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_mazinger_store
-- ----------------------------
DROP TABLE IF EXISTS `t_mazinger_store`;
CREATE TABLE `t_mazinger_store`  (
  `rid` int(11) NOT NULL,
  `type` int(4) NOT NULL,
  `stage` int(8) NOT NULL,
  `level` int(8) NOT NULL,
  `exp` int(11) NOT NULL,
  INDEX `rid_type`(`rid`, `type`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_merlin_magic_book
-- ----------------------------
DROP TABLE IF EXISTS `t_merlin_magic_book`;
CREATE TABLE `t_merlin_magic_book`  (
  `roleID` int(11) NOT NULL DEFAULT 0,
  `occupation` tinyint(4) NOT NULL,
  `level` int(6) NOT NULL DEFAULT 1,
  `level_up_fail_num` int(6) NULL DEFAULT 0,
  `starNum` int(6) NULL DEFAULT 0,
  `starExp` int(6) NULL DEFAULT 0,
  `luckyPoint` int(6) NULL DEFAULT 0,
  `toTicks` datetime NULL DEFAULT '0000-00-00 00:00:00',
  `addTime` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `activeFrozen` int(6) NULL DEFAULT 0,
  `activePalsy` int(6) NULL DEFAULT 0,
  `activeSpeedDown` int(6) NULL DEFAULT 0,
  `activeBlow` int(6) NULL DEFAULT 0,
  `unActiveFrozen` int(6) NULL DEFAULT 0,
  `unActivePalsy` int(6) NULL DEFAULT 0,
  `unActiveSpeedDown` int(6) NULL DEFAULT 0,
  `unActiveBlow` int(6) NULL DEFAULT 0,
  PRIMARY KEY (`roleID`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_mojingexchangeinfo
-- ----------------------------
DROP TABLE IF EXISTS `t_mojingexchangeinfo`;
CREATE TABLE `t_mojingexchangeinfo`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `exchangeid` int(11) NOT NULL DEFAULT 0,
  `exchangenum` int(11) NOT NULL DEFAULT 0,
  `dayid` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`roleid`, `exchangeid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_money
-- ----------------------------
DROP TABLE IF EXISTS `t_money`;
CREATE TABLE `t_money`  (
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0',
  `money` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `realmoney` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `cc` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `giftid` int(11) NOT NULL DEFAULT 0,
  `giftjifen` int(11) NOT NULL DEFAULT 0,
  `points` int(11) NOT NULL DEFAULT 0,
  `specjifen` int(11) NOT NULL DEFAULT 0,
  `everyjifen` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `userid`(`userid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_name_check
-- ----------------------------
DROP TABLE IF EXISTS `t_name_check`;
CREATE TABLE `t_name_check`  (
  `Id` int(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 62 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_npcbuy
-- ----------------------------
DROP TABLE IF EXISTS `t_npcbuy`;
CREATE TABLE `t_npcbuy`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `totalprice` int(11) NOT NULL DEFAULT 0,
  `leftmoney` int(11) NOT NULL DEFAULT 0,
  `moneytype` int(11) NOT NULL DEFAULT 0,
  `buytime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 8760 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_olympics_guess
-- ----------------------------
DROP TABLE IF EXISTS `t_olympics_guess`;
CREATE TABLE `t_olympics_guess`  (
  `roleID` int(11) NULL DEFAULT 0,
  `dayID` int(11) NULL DEFAULT NULL,
  `a1` int(11) NULL DEFAULT -1,
  `a2` int(11) NULL DEFAULT -1,
  `a3` int(11) NULL DEFAULT -1,
  `award1` int(11) NULL DEFAULT 0,
  `award2` int(11) NULL DEFAULT 0,
  `award3` int(11) NULL DEFAULT 0,
  UNIQUE INDEX `idx_guess`(`roleID`, `dayID`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_olympics_shop
-- ----------------------------
DROP TABLE IF EXISTS `t_olympics_shop`;
CREATE TABLE `t_olympics_shop`  (
  `dayID` int(11) NOT NULL DEFAULT 0,
  `shopID` int(11) NOT NULL DEFAULT 0,
  `count` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `idx_olympics`(`dayID`, `shopID`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_onlinenum
-- ----------------------------
DROP TABLE IF EXISTS `t_onlinenum`;
CREATE TABLE `t_onlinenum`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `num` int(11) NOT NULL DEFAULT 0,
  `rectime` datetime NULL,
  `mapnum` char(254) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 13752 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Records of t_onlinenum
-- ----------------------------
INSERT INTO `t_onlinenum` VALUES (13741, 0, '2020-09-18 10:05:06', '');
INSERT INTO `t_onlinenum` VALUES (13742, 0, '2020-09-18 10:07:06', '');
INSERT INTO `t_onlinenum` VALUES (13743, 0, '2020-09-18 10:09:06', '');
INSERT INTO `t_onlinenum` VALUES (13744, 0, '2020-09-18 10:11:06', '');
INSERT INTO `t_onlinenum` VALUES (13745, 0, '2020-09-18 10:15:07', '');
INSERT INTO `t_onlinenum` VALUES (13746, 0, '2020-09-18 10:17:07', '');
INSERT INTO `t_onlinenum` VALUES (13747, 0, '2020-09-18 10:19:07', '');
INSERT INTO `t_onlinenum` VALUES (13748, 0, '2020-09-18 10:21:07', '');
INSERT INTO `t_onlinenum` VALUES (13749, 0, '2020-09-18 10:23:08', '');
INSERT INTO `t_onlinenum` VALUES (13750, 0, '2020-09-18 10:25:08', '');
INSERT INTO `t_onlinenum` VALUES (13751, 0, '2020-09-18 10:27:08', '');

-- ----------------------------
-- Table structure for t_order
-- ----------------------------
DROP TABLE IF EXISTS `t_order`;
CREATE TABLE `t_order`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `order_no` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `order_no`(`order_no`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_ornament
-- ----------------------------
DROP TABLE IF EXISTS `t_ornament`;
CREATE TABLE `t_ornament`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `param1` int(11) NOT NULL DEFAULT 0,
  `param2` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`roleid`, `goodsid`) USING BTREE,
  INDEX `key_roleid`(`roleid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_pets
-- ----------------------------
DROP TABLE IF EXISTS `t_pets`;
CREATE TABLE `t_pets`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `petid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `petname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `pettype` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `feednum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `realivenum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `addtime` datetime NULL,
  `props` char(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `isdel` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `level` int(11) UNSIGNED NOT NULL DEFAULT 1,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_picturejudgeinfo
-- ----------------------------
DROP TABLE IF EXISTS `t_picturejudgeinfo`;
CREATE TABLE `t_picturejudgeinfo`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `roleid` int(11) NOT NULL DEFAULT 0,
  `picturejudgeid` int(11) NOT NULL DEFAULT 0,
  `refercount` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `roleid_picturejudge`(`roleid`, `picturejudgeid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 9040 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_prenames
-- ----------------------------
DROP TABLE IF EXISTS `t_prenames`;
CREATE TABLE `t_prenames`  (
  `name` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `sex` tinyint(6) NOT NULL DEFAULT 0,
  `used` tinyint(6) NOT NULL DEFAULT 0,
  UNIQUE INDEX `name`(`name`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_ptbag
-- ----------------------------
DROP TABLE IF EXISTS `t_ptbag`;
CREATE TABLE `t_ptbag`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `extgridnum` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_pushmessageinfo
-- ----------------------------
DROP TABLE IF EXISTS `t_pushmessageinfo`;
CREATE TABLE `t_pushmessageinfo`  (
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `pushid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `lastlogintime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  PRIMARY KEY (`userid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_qianggoubuy
-- ----------------------------
DROP TABLE IF EXISTS `t_qianggoubuy`;
CREATE TABLE `t_qianggoubuy`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `totalprice` int(11) NOT NULL DEFAULT 0,
  `leftmoney` int(11) NOT NULL DEFAULT 0,
  `qianggouid` int(11) NOT NULL DEFAULT 0,
  `buytime` datetime NULL,
  `actstartday` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `goodsid_qianggouid`(`goodsid`, `qianggouid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_qianggouitem
-- ----------------------------
DROP TABLE IF EXISTS `t_qianggouitem`;
CREATE TABLE `t_qianggouitem`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `itemgroup` int(11) NOT NULL DEFAULT 0,
  `random` int(11) NOT NULL DEFAULT 0,
  `itemid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `origprice` int(11) NOT NULL DEFAULT 0,
  `price` int(11) NOT NULL DEFAULT 0,
  `singlepurchase` int(11) NOT NULL DEFAULT 0,
  `fullpurchase` int(11) NOT NULL DEFAULT 0,
  `daystime` int(11) NOT NULL DEFAULT 0,
  `starttime` datetime NULL,
  `endtime` datetime NULL,
  `istimeover` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_qizhengebuy
-- ----------------------------
DROP TABLE IF EXISTS `t_qizhengebuy`;
CREATE TABLE `t_qizhengebuy`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `totalprice` int(11) NOT NULL DEFAULT 0,
  `leftmoney` int(11) NOT NULL DEFAULT 0,
  `buytime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_reborn_equiphole
-- ----------------------------
DROP TABLE IF EXISTS `t_reborn_equiphole`;
CREATE TABLE `t_reborn_equiphole`  (
  `rid` int(11) NOT NULL,
  `holeid` int(5) NOT NULL,
  `level` int(5) NOT NULL,
  `able` int(11) NOT NULL,
  INDEX `rid_hole`(`rid`, `holeid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_reborn_stamp
-- ----------------------------
DROP TABLE IF EXISTS `t_reborn_stamp`;
CREATE TABLE `t_reborn_stamp`  (
  `rid` int(11) NOT NULL,
  `stamp` char(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `reset` int(11) NULL DEFAULT NULL,
  `use_point` int(11) NULL DEFAULT NULL,
  PRIMARY KEY (`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_reborn_storage
-- ----------------------------
DROP TABLE IF EXISTS `t_reborn_storage`;
CREATE TABLE `t_reborn_storage`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `extgridnum` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_refreshqizhen
-- ----------------------------
DROP TABLE IF EXISTS `t_refreshqizhen`;
CREATE TABLE `t_refreshqizhen`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `oldusermoney` int(11) NOT NULL DEFAULT 0,
  `leftusermoney` int(11) NOT NULL DEFAULT 0,
  `refreshtime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_resourcegetinfo
-- ----------------------------
DROP TABLE IF EXISTS `t_resourcegetinfo`;
CREATE TABLE `t_resourcegetinfo`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `type` int(11) NOT NULL DEFAULT 0,
  `leftcount` int(11) NOT NULL DEFAULT 0,
  `exp` int(11) NOT NULL DEFAULT 0,
  `bandmoney` int(11) NOT NULL DEFAULT 0,
  `mojing` int(11) NOT NULL DEFAULT 0,
  `chengjiu` int(11) NOT NULL DEFAULT 0,
  `shengwang` int(11) NOT NULL DEFAULT 0,
  `zhangong` int(11) NOT NULL DEFAULT 0,
  `bangzuan` int(11) NOT NULL,
  `xinghun` int(11) NOT NULL,
  `hasget` int(11) NOT NULL DEFAULT 0,
  `yuansufenmo` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`roleid`, `type`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_roledata
-- ----------------------------
DROP TABLE IF EXISTS `t_roledata`;
CREATE TABLE `t_roledata`  (
  `rid` int(11) NOT NULL,
  `occu_data` mediumblob NULL,
  `roledataex_ex` mediumblob NULL,
  `roledata4selector` blob NULL,
  PRIMARY KEY (`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = binary ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_rolegmail_record
-- ----------------------------
DROP TABLE IF EXISTS `t_rolegmail_record`;
CREATE TABLE `t_rolegmail_record`  (
  `roleid` int(11) NOT NULL,
  `gmailid` int(11) NOT NULL,
  `mailid` int(11) NULL DEFAULT 0,
  PRIMARY KEY (`roleid`, `gmailid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_roleparams
-- ----------------------------
DROP TABLE IF EXISTS `t_roleparams`;
CREATE TABLE `t_roleparams`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `pname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `pvalue` char(128) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  UNIQUE INDEX `rid_pname`(`rid`, `pname`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_roleparams_2
-- ----------------------------
DROP TABLE IF EXISTS `t_roleparams_2`;
CREATE TABLE `t_roleparams_2`  (
  `rid` int(11) NOT NULL,
  `pname` char(32) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `pvalue` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`rid`, `pname`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = ascii COLLATE = ascii_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_roleparams_char
-- ----------------------------
DROP TABLE IF EXISTS `t_roleparams_char`;
CREATE TABLE `t_roleparams_char`  (
  `rid` int(11) NOT NULL,
  `idx` int(11) NOT NULL,
  `v0` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT '',
  `v1` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT '',
  `v2` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT '',
  `v3` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT '',
  `v4` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT '',
  `v5` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT '',
  `v6` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT '',
  `v7` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT '',
  `v8` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT '',
  `v9` char(128) CHARACTER SET ascii COLLATE ascii_general_ci NULL DEFAULT '',
  PRIMARY KEY (`rid`, `idx`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = ascii COLLATE = ascii_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_roleparams_long
-- ----------------------------
DROP TABLE IF EXISTS `t_roleparams_long`;
CREATE TABLE `t_roleparams_long`  (
  `rid` int(11) NOT NULL,
  `idx` int(11) NOT NULL,
  `v0` bigint(20) NULL DEFAULT 0,
  `v1` bigint(20) NULL DEFAULT 0,
  `v2` bigint(20) NULL DEFAULT 0,
  `v3` bigint(20) NULL DEFAULT 0,
  `v4` bigint(20) NULL DEFAULT 0,
  `v5` bigint(20) NULL DEFAULT 0,
  `v6` bigint(20) NULL DEFAULT 0,
  `v7` bigint(20) NULL DEFAULT 0,
  `v8` bigint(20) NULL DEFAULT 0,
  `v9` bigint(20) NULL DEFAULT 0,
  `v10` bigint(20) NULL DEFAULT 0,
  `v11` bigint(20) NULL DEFAULT 0,
  `v12` bigint(20) NULL DEFAULT 0,
  `v13` bigint(20) NULL DEFAULT 0,
  `v14` bigint(20) NULL DEFAULT 0,
  `v15` bigint(20) NULL DEFAULT 0,
  `v16` bigint(20) NULL DEFAULT 0,
  `v17` bigint(20) NULL DEFAULT 0,
  `v18` bigint(20) NULL DEFAULT 0,
  `v19` bigint(20) NULL DEFAULT 0,
  `v20` bigint(20) NULL DEFAULT 0,
  `v21` bigint(20) NULL DEFAULT 0,
  `v22` bigint(20) NULL DEFAULT 0,
  `v23` bigint(20) NULL DEFAULT 0,
  `v24` bigint(20) NULL DEFAULT 0,
  `v25` bigint(20) NULL DEFAULT 0,
  `v26` bigint(20) NULL DEFAULT 0,
  `v27` bigint(20) NULL DEFAULT 0,
  `v28` bigint(20) NULL DEFAULT 0,
  `v29` bigint(20) NULL DEFAULT 0,
  `v30` bigint(20) NULL DEFAULT 0,
  `v31` bigint(20) NULL DEFAULT 0,
  `v32` bigint(20) NULL DEFAULT 0,
  `v33` bigint(20) NULL DEFAULT 0,
  `v34` bigint(20) NULL DEFAULT 0,
  `v35` bigint(20) NULL DEFAULT 0,
  `v36` bigint(20) NULL DEFAULT 0,
  `v37` bigint(20) NULL DEFAULT 0,
  `v38` bigint(20) NULL DEFAULT 0,
  `v39` bigint(20) NULL DEFAULT 0,
  PRIMARY KEY (`rid`, `idx`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = ascii COLLATE = ascii_bin ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_roles
-- ----------------------------
DROP TABLE IF EXISTS `t_roles`;
CREATE TABLE `t_roles`  (
  `rid` int(11) NOT NULL AUTO_INCREMENT,
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `rname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `sex` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `occupation` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `level` smallint(6) UNSIGNED NOT NULL DEFAULT 1,
  `pic` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `faction` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `money1` int(11) NOT NULL DEFAULT 0,
  `money2` int(11) NOT NULL DEFAULT 0,
  `experience` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `pkmode` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `pkvalue` int(11) NOT NULL DEFAULT 0,
  `position` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '-1:0:-1:-1',
  `regtime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `lasttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `isdel` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `deltime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `predeltime` datetime NULL DEFAULT NULL,
  `bagnum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `othername` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `main_quick_keys` char(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `other_quick_keys` char(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `loginnum` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `leftfightsecs` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `horseid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `petid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `interpower` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `totalonlinesecs` int(11) NOT NULL DEFAULT 0,
  `antiaddictionsecs` int(11) NOT NULL DEFAULT 0,
  `logofftime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `biguantime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `yinliang` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `total_jingmai_exp` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `jingmai_exp_num` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `lasthorseid` int(11) NOT NULL DEFAULT 0,
  `skillid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `autolife` int(11) NOT NULL DEFAULT 70,
  `automagic` int(11) NOT NULL DEFAULT 50,
  `numskillid` int(11) NOT NULL DEFAULT 0,
  `maintaskid` int(11) NOT NULL DEFAULT 0,
  `pkpoint` int(11) NOT NULL DEFAULT 0,
  `lianzhan` int(11) NOT NULL DEFAULT 0,
  `killboss` int(11) NOT NULL DEFAULT 0,
  `equipjifen` int(11) NOT NULL DEFAULT 0,
  `xueweinum` int(11) NOT NULL DEFAULT 0,
  `skilllearnednum` int(11) NOT NULL DEFAULT 0,
  `horsejifen` int(11) NOT NULL DEFAULT 0,
  `battlenamestart` bigint(20) NOT NULL DEFAULT 0,
  `battlenameindex` int(11) NOT NULL DEFAULT 0,
  `cztaskid` int(11) NOT NULL DEFAULT 0,
  `battlenum` int(11) NOT NULL DEFAULT 0,
  `heroindex` int(11) NOT NULL DEFAULT 0,
  `logindayid` int(11) NOT NULL DEFAULT 0,
  `logindaynum` int(11) NOT NULL DEFAULT 0,
  `zoneid` int(11) NOT NULL DEFAULT 0,
  `bhname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `bhverify` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `bhzhiwu` int(11) NOT NULL DEFAULT 0,
  `chenghao` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `bgdayid1` int(11) NOT NULL DEFAULT 0,
  `bgmoney` int(11) NOT NULL DEFAULT 0,
  `bgdayid2` int(11) NOT NULL DEFAULT 0,
  `bggoods` int(11) NOT NULL DEFAULT 0,
  `banggong` int(11) NOT NULL DEFAULT 0,
  `huanghou` int(11) NOT NULL DEFAULT 0,
  `jiebiaodayid` int(11) NOT NULL DEFAULT 0,
  `jiebiaonum` int(11) NOT NULL DEFAULT 0,
  `username` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `lastmailid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `onceawardflag` bigint(11) UNSIGNED NOT NULL DEFAULT 0,
  `banchat` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `banlogin` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `isflashplayer` int(11) NOT NULL DEFAULT 0,
  `changelifecount` int(11) NOT NULL DEFAULT 0,
  `admiredcount` int(11) NOT NULL DEFAULT 0,
  `combatforce` int(11) NOT NULL DEFAULT 0,
  `autoassignpropertypoint` int(11) NOT NULL DEFAULT 1,
  `vipawardflag` int(11) NOT NULL DEFAULT 0,
  `store_yinliang` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `store_money` bigint(20) UNSIGNED NOT NULL DEFAULT 0,
  `magic_sword_param` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `fluorescent_point` int(11) NOT NULL DEFAULT 0,
  `ban_trade_to_ticks` bigint(20) NOT NULL DEFAULT 0,
  `juntuanzhiwu` tinyint(4) NOT NULL DEFAULT 0,
  `huiji` int(11) NOT NULL DEFAULT 0,
  `huijiexp` int(11) NOT NULL DEFAULT 0,
  `armor` int(11) NOT NULL DEFAULT 0,
  `armorexp` int(11) NOT NULL DEFAULT 0,
  `bianshen` int(11) NOT NULL DEFAULT 0,
  `bianshenexp` int(11) NOT NULL DEFAULT 0,
  `reborn_bagnum` int(11) NOT NULL DEFAULT 0,
  `reborn_isshow` tinyint(4) NOT NULL DEFAULT 0,
  `reborn_isshow_model` tinyint(4) NOT NULL DEFAULT 0,
  `zhanduiid` int(11) NOT NULL DEFAULT 0,
  `zhanduizhiwu` tinyint(4) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid`(`rid`) USING BTREE,
  UNIQUE INDEX `rname_zoneid`(`rname`, `zoneid`) USING BTREE,
  INDEX `userid`(`userid`) USING BTREE,
  INDEX `idx_faction`(`faction`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 200000 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_secondpassword
-- ----------------------------
DROP TABLE IF EXISTS `t_secondpassword`;
CREATE TABLE `t_secondpassword`  (
  `userid` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `secpwd` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`userid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_server_days
-- ----------------------------
DROP TABLE IF EXISTS `t_server_days`;
CREATE TABLE `t_server_days`  (
  `dayid` int(11) NOT NULL,
  `cdate` date NOT NULL,
  `worldlevel` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`dayid`) USING BTREE,
  INDEX `cdate`(`cdate`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = latin1 COLLATE = latin1_swedish_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_seven_day_act
-- ----------------------------
DROP TABLE IF EXISTS `t_seven_day_act`;
CREATE TABLE `t_seven_day_act`  (
  `roleid` int(11) NOT NULL DEFAULT 0,
  `act_type` int(11) NOT NULL DEFAULT 0,
  `id` int(11) NOT NULL DEFAULT 0,
  `award_flag` int(11) NOT NULL DEFAULT 0,
  `param1` int(11) NOT NULL DEFAULT 0,
  `param2` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`roleid`, `act_type`, `id`) USING BTREE,
  INDEX `key_roleid`(`roleid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_shengxiaoguesshist
-- ----------------------------
DROP TABLE IF EXISTS `t_shengxiaoguesshist`;
CREATE TABLE `t_shengxiaoguesshist`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `rname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `zoneid` int(11) NOT NULL DEFAULT 0,
  `guesskey` int(11) NOT NULL DEFAULT 0,
  `mortgage` int(11) NOT NULL DEFAULT 0,
  `resultkey` int(11) NOT NULL DEFAULT 0,
  `gainnum` int(11) NOT NULL DEFAULT 0,
  `leftmortgage` int(11) NOT NULL DEFAULT 0,
  `guesstime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_shenjifuwen
-- ----------------------------
DROP TABLE IF EXISTS `t_shenjifuwen`;
CREATE TABLE `t_shenjifuwen`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `sjID` int(11) NOT NULL DEFAULT 0,
  `level` tinyint(1) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_sjID`(`rid`, `sjID`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_skills
-- ----------------------------
DROP TABLE IF EXISTS `t_skills`;
CREATE TABLE `t_skills`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `skillid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `skilllevel` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `usednum` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `rid_skillid`(`rid`, `skillid`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 8326 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_special_activity
-- ----------------------------
DROP TABLE IF EXISTS `t_special_activity`;
CREATE TABLE `t_special_activity`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `groupid` int(11) NOT NULL DEFAULT 0,
  `actid` int(11) NOT NULL DEFAULT 0,
  `purchaseNum` int(11) NOT NULL DEFAULT 0,
  `countNum` int(11) NOT NULL DEFAULT 0,
  `active` smallint(6) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_groupid_actid`(`rid`, `groupid`, `actid`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_special_priority_activity
-- ----------------------------
DROP TABLE IF EXISTS `t_special_priority_activity`;
CREATE TABLE `t_special_priority_activity`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `tequanid` int(11) NOT NULL DEFAULT 0,
  `actid` int(11) NOT NULL DEFAULT 0,
  `purchaseNum` int(11) NOT NULL DEFAULT 0,
  `countNum` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_tequanid_actid`(`rid`, `tequanid`, `actid`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_spread_award
-- ----------------------------
DROP TABLE IF EXISTS `t_spread_award`;
CREATE TABLE `t_spread_award`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `zoneID` int(11) NULL DEFAULT NULL,
  `roleID` int(11) NULL DEFAULT NULL,
  `type` tinyint(1) NULL DEFAULT NULL,
  `state` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `dex_spread`(`zoneID`, `roleID`, `type`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_starconstellationinfo
-- ----------------------------
DROP TABLE IF EXISTS `t_starconstellationinfo`;
CREATE TABLE `t_starconstellationinfo`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `roleid` int(11) NOT NULL DEFAULT 0,
  `starsiteid` int(11) NOT NULL DEFAULT 0,
  `starslotid` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `roleid_starconstellation`(`roleid`, `starsiteid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 42813 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_talent
-- ----------------------------
DROP TABLE IF EXISTS `t_talent`;
CREATE TABLE `t_talent`  (
  `roleID` int(11) NOT NULL DEFAULT 0,
  `tatalCount` smallint(3) NOT NULL DEFAULT 0,
  `exp` bigint(20) NOT NULL DEFAULT 0,
  `zoneID` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`roleID`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_talent_effect
-- ----------------------------
DROP TABLE IF EXISTS `t_talent_effect`;
CREATE TABLE `t_talent_effect`  (
  `roleID` int(11) NOT NULL DEFAULT 0,
  `talentType` tinyint(1) NOT NULL DEFAULT 0,
  `effectID` int(11) NOT NULL DEFAULT 0,
  `effectLevel` tinyint(1) NOT NULL DEFAULT 0,
  `zoneID` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `roleID_effectID`(`roleID`, `effectID`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_talent_log
-- ----------------------------
DROP TABLE IF EXISTS `t_talent_log`;
CREATE TABLE `t_talent_log`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `zoneID` int(11) NOT NULL DEFAULT 0,
  `roleID` int(11) NOT NULL DEFAULT 0,
  `logType` int(11) NOT NULL DEFAULT 0,
  `logValue` bigint(20) NOT NULL DEFAULT 0,
  `logTime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `zone_type_time`(`zoneID`, `logType`, `logTime`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 29342 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_tarot
-- ----------------------------
DROP TABLE IF EXISTS `t_tarot`;
CREATE TABLE `t_tarot`  (
  `roleid` int(20) NOT NULL,
  `tarotinfo` mediumtext CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
  `kingbuff` mediumtext CHARACTER SET ascii COLLATE ascii_bin NOT NULL,
  PRIMARY KEY (`roleid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_tasks
-- ----------------------------
DROP TABLE IF EXISTS `t_tasks`;
CREATE TABLE `t_tasks`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `taskid` int(11) NOT NULL DEFAULT 0,
  `rid` int(11) NOT NULL DEFAULT 0,
  `focus` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `value1` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `value2` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `isdel` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `addtime` datetime NULL,
  `starlevel` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 12053 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_taskslog
-- ----------------------------
DROP TABLE IF EXISTS `t_taskslog`;
CREATE TABLE `t_taskslog`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `taskid` int(11) NOT NULL DEFAULT 0,
  `count` int(11) UNSIGNED NOT NULL DEFAULT 0,
  UNIQUE INDEX `taskid_rid`(`rid`, `taskid`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_tempitem
-- ----------------------------
DROP TABLE IF EXISTS `t_tempitem`;
CREATE TABLE `t_tempitem`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `uid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `rid` int(11) NOT NULL,
  `addmoney` int(11) NOT NULL DEFAULT 0,
  `itemid` int(11) NOT NULL DEFAULT 0,
  `chargetime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `isdel` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_tempmoney
-- ----------------------------
DROP TABLE IF EXISTS `t_tempmoney`;
CREATE TABLE `t_tempmoney`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `cc` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `uid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `rid` int(11) NOT NULL DEFAULT 0,
  `addmoney` int(11) NOT NULL DEFAULT 0,
  `itemid` int(11) NOT NULL DEFAULT 0,
  `budanflag` int(11) NOT NULL DEFAULT 0,
  `chargetime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 602 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_ten
-- ----------------------------
DROP TABLE IF EXISTS `t_ten`;
CREATE TABLE `t_ten`  (
  `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `serverID` int(11) NOT NULL,
  `uID` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `roleID` int(11) NOT NULL,
  `giftID` int(11) NOT NULL,
  `updatetime` datetime NULL,
  `state` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `state_idx`(`state`) USING BTREE,
  INDEX `only_idx`(`uID`, `giftID`, `state`) USING BTREE,
  INDEX `day_idx`(`uID`, `giftID`, `updatetime`, `state`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_treasure_log
-- ----------------------------
DROP TABLE IF EXISTS `t_treasure_log`;
CREATE TABLE `t_treasure_log`  (
  `time` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  `role` int(11) NOT NULL DEFAULT 0,
  `dice` int(11) NOT NULL DEFAULT 0,
  `superdice` int(11) NOT NULL DEFAULT 0,
  `movenum` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`time`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_usedlipinma
-- ----------------------------
DROP TABLE IF EXISTS `t_usedlipinma`;
CREATE TABLE `t_usedlipinma`  (
  `lipinma` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `huodongid` int(11) NOT NULL DEFAULT 0,
  `ptid` int(11) NOT NULL DEFAULT 0,
  `rid` int(11) NOT NULL DEFAULT 0,
  INDEX `rid`(`rid`) USING BTREE,
  INDEX `huodongid`(`huodongid`) USING BTREE,
  INDEX `ptid`(`ptid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_usemoney_log
-- ----------------------------
DROP TABLE IF EXISTS `t_usemoney_log`;
CREATE TABLE `t_usemoney_log`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `DBId` int(11) NULL DEFAULT NULL,
  `userid` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `ObjName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `optFrom` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `currEnvName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `tarEnvName` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `optType` char(6) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `optTime` datetime NULL DEFAULT NULL,
  `optAmount` int(11) NULL DEFAULT NULL,
  `zoneID` int(11) NULL DEFAULT NULL,
  `optSurplus` int(11) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `DBId`(`DBId`) USING BTREE,
  INDEX `tarEnvName`(`tarEnvName`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 75609 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_user_active_info
-- ----------------------------
DROP TABLE IF EXISTS `t_user_active_info`;
CREATE TABLE `t_user_active_info`  (
  `Account` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `createTime` date NOT NULL,
  `seriesLoginCount` int(11) NOT NULL DEFAULT 0,
  `lastSeriesLoginTime` date NOT NULL,
  PRIMARY KEY (`Account`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_user_return
-- ----------------------------
DROP TABLE IF EXISTS `t_user_return`;
CREATE TABLE `t_user_return`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `activityID` int(11) NOT NULL DEFAULT 0,
  `activityDay` date NOT NULL DEFAULT '1900-00-00',
  `zoneID` int(11) NOT NULL DEFAULT 0,
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `vip` int(11) NOT NULL DEFAULT 0,
  `level` int(11) NOT NULL DEFAULT 0,
  `logTime` date NOT NULL DEFAULT '1900-01-01',
  `checkState` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_user_return_award
-- ----------------------------
DROP TABLE IF EXISTS `t_user_return_award`;
CREATE TABLE `t_user_return_award`  (
  `activityID` int(11) NOT NULL DEFAULT 0,
  `activityDay` date NOT NULL DEFAULT '1900-01-01',
  `zoneID` int(11) NOT NULL DEFAULT 0,
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `type` tinyint(1) NOT NULL DEFAULT 0,
  `state` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  UNIQUE INDEX `time_uid_zoneID_idx`(`activityID`, `activityDay`, `userid`, `zoneID`, `type`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_userstat
-- ----------------------------
DROP TABLE IF EXISTS `t_userstat`;
CREATE TABLE `t_userstat`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `userid` char(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0',
  `serverid` int(11) NOT NULL DEFAULT 0,
  `eventid` int(11) NOT NULL DEFAULT 0,
  `rectime` int(11) NOT NULL DEFAULT 0,
  `loginnum` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  UNIQUE INDEX `userid_serverid`(`userid`, `serverid`) USING BTREE,
  INDEX `eventid`(`eventid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_vipdailydata
-- ----------------------------
DROP TABLE IF EXISTS `t_vipdailydata`;
CREATE TABLE `t_vipdailydata`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `prioritytype` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `dayid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `usedtimes` int(11) UNSIGNED NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_prioritytype`(`rid`, `prioritytype`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_wanmota
-- ----------------------------
DROP TABLE IF EXISTS `t_wanmota`;
CREATE TABLE `t_wanmota`  (
  `roleID` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `roleName` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `flushTime` bigint(20) NOT NULL DEFAULT 0,
  `passLayerCount` int(11) NOT NULL DEFAULT 0,
  `sweepLayer` int(11) NULL DEFAULT 0,
  `sweepReward` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL,
  `sweepBeginTime` bigint(20) NULL DEFAULT 0,
  PRIMARY KEY (`roleID`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_warning
-- ----------------------------
DROP TABLE IF EXISTS `t_warning`;
CREATE TABLE `t_warning`  (
  `Id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `usedmoney` int(11) NOT NULL DEFAULT 0,
  `goodsmoney` int(11) NOT NULL DEFAULT 0,
  `warningtime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 343047 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_weboldplayer
-- ----------------------------
DROP TABLE IF EXISTS `t_weboldplayer`;
CREATE TABLE `t_weboldplayer`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `v0` int(11) NOT NULL DEFAULT 0,
  `v1` int(11) NOT NULL DEFAULT 0,
  `v2` int(11) NOT NULL DEFAULT 0,
  `v3` int(11) NOT NULL DEFAULT 0,
  `v4` int(11) NOT NULL DEFAULT 0,
  `addday` datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  UNIQUE INDEX `rid_day`(`rid`, `addday`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_wings
-- ----------------------------
DROP TABLE IF EXISTS `t_wings`;
CREATE TABLE `t_wings`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `rname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `occupation` tinyint(4) NOT NULL,
  `wingid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `forgeLevel` int(11) NOT NULL,
  `addtime` datetime NULL,
  `isdel` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `failednum` int(11) NOT NULL DEFAULT 0,
  `equiped` int(11) NOT NULL DEFAULT 0,
  `starexp` int(11) NOT NULL DEFAULT 0,
  `zhulingnum` int(11) NOT NULL DEFAULT 0,
  `zhuhunnum` int(11) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `rid_idx`(`rid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 840 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_yabiao
-- ----------------------------
DROP TABLE IF EXISTS `t_yabiao`;
CREATE TABLE `t_yabiao`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `yabiaoid` int(11) NOT NULL DEFAULT 0,
  `starttime` datetime NOT NULL DEFAULT '1900-01-01 12:00:00',
  `state` int(11) NOT NULL DEFAULT 0,
  `lineid` int(11) NOT NULL DEFAULT 0,
  `toubao` int(11) NOT NULL DEFAULT 0,
  `yabiaodayid` int(11) NOT NULL DEFAULT 0,
  `yabiaonum` int(11) NOT NULL DEFAULT 0,
  `takegoods` int(11) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_yangguangbkdailydata
-- ----------------------------
DROP TABLE IF EXISTS `t_yangguangbkdailydata`;
CREATE TABLE `t_yangguangbkdailydata`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `jifen` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `dayid` int(11) UNSIGNED NOT NULL DEFAULT 0,
  `awardhistory` int(11) UNSIGNED NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_unique`(`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_yaosaiboss
-- ----------------------------
DROP TABLE IF EXISTS `t_yaosaiboss`;
CREATE TABLE `t_yaosaiboss`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `bossID` int(11) NOT NULL DEFAULT 0,
  `bosslife` int(11) NOT NULL DEFAULT 0,
  `deadtime` datetime NOT NULL DEFAULT '2000-01-01 00:00:00',
  PRIMARY KEY (`rid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_yaosaiboss_fight
-- ----------------------------
DROP TABLE IF EXISTS `t_yaosaiboss_fight`;
CREATE TABLE `t_yaosaiboss_fight`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `otherrid` int(11) NOT NULL DEFAULT 0,
  `otherrname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `invitetype` int(11) NOT NULL DEFAULT 0,
  `fightlife` int(11) NOT NULL DEFAULT 0,
  INDEX `rid_otherrid`(`rid`, `otherrid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_yaosaimission
-- ----------------------------
DROP TABLE IF EXISTS `t_yaosaimission`;
CREATE TABLE `t_yaosaimission`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `siteid` int(11) NOT NULL DEFAULT 0,
  `missionid` int(11) NOT NULL DEFAULT 0,
  `state` int(11) NOT NULL DEFAULT 0,
  `zhipaijingling` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `starttime` datetime NOT NULL DEFAULT '0000-00-00 00:00:00',
  PRIMARY KEY (`rid`, `siteid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_yinliangbuy
-- ----------------------------
DROP TABLE IF EXISTS `t_yinliangbuy`;
CREATE TABLE `t_yinliangbuy`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `totalprice` int(11) NOT NULL DEFAULT 0,
  `leftyinliang` int(11) NOT NULL DEFAULT 0,
  `buytime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_yinpiaobuy
-- ----------------------------
DROP TABLE IF EXISTS `t_yinpiaobuy`;
CREATE TABLE `t_yinpiaobuy`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `goodsnum` int(11) NOT NULL DEFAULT 0,
  `totalprice` int(11) NOT NULL DEFAULT 0,
  `leftyinpiao` int(11) NOT NULL DEFAULT 0,
  `buytime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_yueduchoujianghist
-- ----------------------------
DROP TABLE IF EXISTS `t_yueduchoujianghist`;
CREATE TABLE `t_yueduchoujianghist`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `rname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `zoneid` int(11) NOT NULL DEFAULT 0,
  `gaingoodsid` int(11) NOT NULL DEFAULT 0,
  `gaingoodsnum` int(11) NOT NULL DEFAULT 0,
  `gaingold` int(11) NOT NULL DEFAULT 0,
  `gainyinliang` int(11) NOT NULL DEFAULT 0,
  `gainexp` int(11) NOT NULL DEFAULT 0,
  `operationtime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_zajindanhist
-- ----------------------------
DROP TABLE IF EXISTS `t_zajindanhist`;
CREATE TABLE `t_zajindanhist`  (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `rid` int(11) NOT NULL DEFAULT 0,
  `rname` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `zoneid` int(11) NOT NULL DEFAULT 0,
  `timesselected` int(11) NOT NULL DEFAULT 0,
  `usedyuanbao` int(11) NOT NULL DEFAULT 0,
  `usedjindan` int(11) NOT NULL DEFAULT 0,
  `gaingoodsid` int(11) NOT NULL DEFAULT 0,
  `gaingoodsnum` int(11) NOT NULL DEFAULT 0,
  `gaingold` int(11) NOT NULL DEFAULT 0,
  `gainyinliang` int(11) NOT NULL DEFAULT 0,
  `gainexp` int(11) NOT NULL DEFAULT 0,
  `strprop` char(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `operationtime` datetime NULL,
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `rid_idx`(`rid`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 518 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_zhanmengshijian
-- ----------------------------
DROP TABLE IF EXISTS `t_zhanmengshijian`;
CREATE TABLE `t_zhanmengshijian`  (
  `pkId` int(11) NOT NULL AUTO_INCREMENT,
  `bhId` int(11) NOT NULL,
  `shijianType` int(11) NOT NULL,
  `roleName` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `createTime` datetime NULL,
  `subValue1` int(11) NOT NULL,
  `subValue2` int(11) NOT NULL,
  `subValue3` int(11) NOT NULL,
  `subSzValue1` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  PRIMARY KEY (`pkId`) USING BTREE,
  INDEX `idx_t_zhanmengshijian_bhId`(`bhId`) USING BTREE,
  INDEX `idx_t_zhanmengshijian_createTime`(`createTime`) USING BTREE,
  INDEX `rname_idx`(`roleName`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 90201 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_zhengba_pk_log
-- ----------------------------
DROP TABLE IF EXISTS `t_zhengba_pk_log`;
CREATE TABLE `t_zhengba_pk_log`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `month` int(11) NOT NULL DEFAULT 0,
  `day` tinyint(4) UNSIGNED NOT NULL DEFAULT 0,
  `rid1` int(11) NOT NULL DEFAULT 0,
  `zoneid1` int(11) NOT NULL DEFAULT 0,
  `rname1` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ismirror1` tinyint(4) NOT NULL DEFAULT 0,
  `rid2` int(11) NOT NULL DEFAULT 0,
  `zoneid2` int(11) NOT NULL DEFAULT 0,
  `rname2` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ismirror2` tinyint(4) NOT NULL DEFAULT 0,
  `result` tinyint(4) NOT NULL DEFAULT 0,
  `upgrade` tinyint(4) NOT NULL DEFAULT 0,
  `starttime` datetime NULL,
  `endtime` datetime NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `key_month`(`month`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_zhengba_support_flag
-- ----------------------------
DROP TABLE IF EXISTS `t_zhengba_support_flag`;
CREATE TABLE `t_zhengba_support_flag`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `month` int(11) NOT NULL DEFAULT 0,
  `rank_of_day` tinyint(4) NOT NULL DEFAULT 0,
  `from_rid` int(11) NOT NULL DEFAULT 0,
  `from_zoneid` int(11) NOT NULL DEFAULT 0,
  `from_rolename` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `from_serverid` int(11) NOT NULL DEFAULT 0,
  `support_type` tinyint(4) NOT NULL DEFAULT 0,
  `to_union_group` int(11) NOT NULL DEFAULT 0,
  `to_group` tinyint(4) NOT NULL DEFAULT 0,
  `is_award` tinyint(4) NOT NULL DEFAULT 0,
  `time` datetime NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `key_f_rid`(`from_rid`) USING BTREE,
  INDEX `key_t_union_group`(`to_union_group`) USING BTREE,
  INDEX `key_month`(`month`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_zhengba_support_log
-- ----------------------------
DROP TABLE IF EXISTS `t_zhengba_support_log`;
CREATE TABLE `t_zhengba_support_log`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `month` int(11) NOT NULL DEFAULT 0,
  `rank_of_day` tinyint(4) NOT NULL DEFAULT 0,
  `from_rid` int(11) NOT NULL DEFAULT 0,
  `from_zoneid` int(11) NOT NULL DEFAULT 0,
  `from_rolename` char(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `from_serverid` int(11) NOT NULL DEFAULT 0,
  `support_type` tinyint(4) NOT NULL DEFAULT 0,
  `to_union_group` int(11) NOT NULL DEFAULT 0,
  `to_group` tinyint(4) NOT NULL DEFAULT 0,
  `time` datetime NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `key_f_rid`(`from_rid`) USING BTREE,
  INDEX `key_t_union_group`(`to_union_group`) USING BTREE,
  INDEX `key_month`(`month`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Table structure for t_zuoqi
-- ----------------------------
DROP TABLE IF EXISTS `t_zuoqi`;
CREATE TABLE `t_zuoqi`  (
  `rid` int(11) NOT NULL DEFAULT 0,
  `goodsid` int(11) NOT NULL DEFAULT 0,
  `isnew` tinyint(4) NOT NULL DEFAULT 0,
  UNIQUE INDEX `rid_goods`(`rid`, `goodsid`) USING BTREE
) ENGINE = MyISAM CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Procedure structure for yjcard
-- ----------------------------
DROP PROCEDURE IF EXISTS `yjcard`;
delimiter ;;
CREATE DEFINER=`root`@`localhost` PROCEDURE `yjcard`(IN name varchar(20),IN rmb int)
BEGIN
#Routine body goes here...
INSERT INTO `t_tempmoney` ( `cc`, `uid`, `addmoney`, `itemid`, `chargetime` ) VALUES ( CONCAT( LEFT ( upper( md5( CONCAT( 'jOU81>.fjoeanl3fw16d21f.*', LEFT (upper(md5(now())), 8), 'YY', ( SELECT userid FROM t_roles WHERE rname = name ), '3sl3e5.', rmb, '=', now()))), 24 ), LEFT (upper(md5(now())), 8)), ( SELECT userid FROM t_roles WHERE rname = name ), rmb, 0, now());
INSERT INTO `t_inputlog` (`amount`, `u`, `rid`,`order_no`, `cporder_no`, `time`, `sign`, `inputtime`, `result`, `zoneid` ) VALUES  (rmb,  (select userid from t_roles where rname = name), ( SELECT rid FROM t_roles WHERE rname = name ),'www', 'yjcard', 'com', '', now(), 'success', (select zoneid from t_roles where rname = name));
INSERT INTO `t_tempmoney` ( `cc`, `uid`, `addmoney`, `itemid`, `chargetime` ) VALUES ( CONCAT( LEFT ( upper( md5( CONCAT( 'jOU81>.fjoeanl3fw16d21f.*', LEFT (upper(md5(now())), 8), 'YY', ( SELECT userid FROM t_roles WHERE rname = name ), '3sl3e5.', '1', '=', now()))), 24 ), LEFT (upper(md5(now())), 8)), ( SELECT userid FROM t_roles WHERE rname = name ), '1', 0, now());

END
;;
delimiter ;

SET FOREIGN_KEY_CHECKS = 1;
