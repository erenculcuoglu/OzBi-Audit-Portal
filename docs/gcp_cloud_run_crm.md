# OzBI Portal CRM - Google Cloud Run Deployment Guide

This guide details how to deploy **OzBI Portal CRM** (.NET 8 Blazor Server) to **Google Cloud Run** with enterprise security, automatic HTTPS/SSL, and $0/low cost.

---

## Prerequisites
- Google Cloud Project: `ozbi-login`
- Dockerfile & .dockerignore (Already created in project root)

---

## Deployment Steps via Google Cloud Console

### Option 1: Direct Cloud Run Function / Source Deployment
1. Open **[console.cloud.google.com/run](https://console.cloud.google.com/run)**
2. Click **`+ Deploy container`** -> **`Continuously deploy from a repository`** (Connect to your GitHub repo `erenculcuoglu/OzBi-Audit-Portal`).
3. Select **Build Type**: `Dockerfile` (Path: `/Dockerfile`).
4. **Service name**: `ozbi-portal-crm`
5. **Region**: `europe-west1` (Belgium / Frankfurt)
6. **Authentication**: Select **`Allow unauthenticated invocations`** (Public access for portal users).
7. Click **`CREATE`**!

---

## Enterprise Security Benefits Achieved
- **gVisor Sandboxing**: Hardware-isolated execution for customer SQL & AI stores.
- **Auto SSL**: Free Google-managed TLS 1.3 certificate for your custom domain.
- **Zero Idle Sleep**: Instant response with automatic zero-scaling when idle.
