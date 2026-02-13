/* Patient records query */
CREATE INDEX IDX_HSBA_MABN
ON HSBA (MABN);

/* Doctor workload query */
CREATE INDEX IDX_HSBA_MABS
ON HSBA (MABS);


/* =========================
   HSBA_DV
   ========================= */

/* Service lookup by type */
CREATE INDEX IDX_HSBADV_LOAIDV
ON HSBA_DV (LOAIDV);

/* Service lookup by date */
CREATE INDEX IDX_HSBADV_NGAYDV
ON HSBA_DV (NGAYDV);


/* =========================
   THONGBAO
   ========================= */

/* OLS filtering - composite index */
CREATE INDEX IDX_THONGBAO_NOIDUNG
ON THONGBAO (NOIDUNG);


CREATE INDEX IDX_THONGBAO_DIADIEM
ON THONGBAO (DIADIEM);
 
/* =========================
   DONTHUOC
   ========================= */

/* Lookup by drug name */
CREATE INDEX IDX_DONTHUOC_TENTHUOC
ON DONTHUOC (TENTHUOC);

